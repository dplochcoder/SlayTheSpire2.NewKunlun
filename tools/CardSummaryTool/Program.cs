using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SmartFormat;
using SmartFormat.Core.Extensions;

const string outputFileName = "CARD_SUMMARY.md";
string repositoryRoot = FindRepositoryRoot(args.FirstOrDefault() ?? AppContext.BaseDirectory);
string cardsDirectory = Path.Combine(repositoryRoot, "NewKunlun", "NewKunlunCode", "Cards");
string outputPath = Path.Combine(repositoryRoot, "NewKunlun", outputFileName);
string projectAssemblyPath =
    args.ElementAtOrDefault(1) ?? FindNewestProjectAssembly(repositoryRoot);

if (!Directory.Exists(cardsDirectory))
{
    Console.Error.WriteLine($"Card source directory does not exist: {cardsDirectory}");
    return 1;
}
if (!File.Exists(projectAssemblyPath))
{
    Console.Error.WriteLine(
        $"The compiled mod assembly does not exist: {projectAssemblyPath}\n"
            + "Build NewKunlun first, or pass its assembly path as the second argument."
    );
    return 1;
}

DateTime assemblyWriteTime = File.GetLastWriteTimeUtc(projectAssemblyPath);
string? newerSourcePath = Directory
    .EnumerateFiles(
        Path.Combine(repositoryRoot, "NewKunlun", "NewKunlunCode"),
        "*.cs",
        SearchOption.AllDirectories
    )
    .FirstOrDefault(path => File.GetLastWriteTimeUtc(path) > assemblyWriteTime);
if (newerSourcePath is not null)
{
    Console.WriteLine(
        $"The compiled mod assembly is older than {Path.GetRelativePath(repositoryRoot, newerSourcePath)}; rebuilding NewKunlun."
    );
    if (!TryBuildProject(repositoryRoot, projectAssemblyPath))
        return 1;
}

var sourceCards = ReadSourceCards(Path.GetDirectoryName(outputPath)!, cardsDirectory);
var projectAssembly = Assembly.LoadFrom(Path.GetFullPath(projectAssemblyPath));
var smartFormatter = CreateGameSmartFormatter(projectAssembly);
var cards = ReadCards(projectAssembly, sourceCards, smartFormatter).ToArray();

if (cards.Length == 0)
{
    Console.Error.WriteLine("No localized NewKunlunCard classes were found in the project build.");
    return 1;
}

File.WriteAllText(outputPath, RenderMarkdown(cards), new UTF8Encoding(false));
Console.WriteLine($"Wrote {cards.Length} cards to {outputPath}.");
return 0;

static bool TryBuildProject(string repositoryRoot, string assemblyPath)
{
    string configuration = new DirectoryInfo(Path.GetDirectoryName(assemblyPath)!).Name;
    if (configuration.Equals("publish", StringComparison.OrdinalIgnoreCase))
        configuration = new DirectoryInfo(Path.GetDirectoryName(assemblyPath)!).Parent!.Name;

    var startInfo = new ProcessStartInfo("dotnet") { UseShellExecute = false };
    startInfo.ArgumentList.Add("build");
    startInfo.ArgumentList.Add(Path.Combine(repositoryRoot, "NewKunlun", "NewKunlun.csproj"));
    startInfo.ArgumentList.Add("--configuration");
    startInfo.ArgumentList.Add(configuration);
    startInfo.ArgumentList.Add("--no-restore");
    startInfo.ArgumentList.Add("-p:SkipCardSummary=true");
    startInfo.ArgumentList.Add("-p:BuildProjectReferences=false");

    using var process = Process.Start(startInfo);
    process?.WaitForExit();
    if (process?.ExitCode == 0)
        return true;

    Console.Error.WriteLine(
        "Could not rebuild NewKunlun. Build the mod project successfully, then rerun CardSummaryTool."
    );
    return false;
}

static string FindNewestProjectAssembly(string repositoryRoot)
{
    string buildRoot = Path.Combine(repositoryRoot, "NewKunlun", ".godot", "mono", "temp", "bin");
    return Directory.Exists(buildRoot)
        ? Directory
            .EnumerateFiles(buildRoot, "NewKunlun.dll", SearchOption.AllDirectories)
            .OrderByDescending(File.GetLastWriteTimeUtc)
            .FirstOrDefault()
            ?? Path.Combine(buildRoot, "Debug", "NewKunlun.dll")
        : Path.Combine(buildRoot, "Debug", "NewKunlun.dll");
}

static string FindRepositoryRoot(string startPath)
{
    var current = new DirectoryInfo(Path.GetFullPath(startPath));
    if (!current.Exists && current.Parent is not null)
        current = current.Parent;

    while (current is not null)
    {
        if (
            Directory.Exists(Path.Combine(current.FullName, ".git"))
            || File.Exists(Path.Combine(current.FullName, "NewKunlun.sln"))
        )
            return current.FullName;

        current = current.Parent;
    }

    throw new DirectoryNotFoundException(
        $"Could not find the repository root starting from '{startPath}'."
    );
}

static IReadOnlyDictionary<string, SourceCardInfo> ReadSourceCards(
    string linkRoot,
    string cardsDirectory
)
{
    var cards = new Dictionary<string, SourceCardInfo>(StringComparer.Ordinal);
    foreach (
        string sourcePath in Directory.EnumerateFiles(
            cardsDirectory,
            "*.cs",
            SearchOption.AllDirectories
        )
    )
    {
        var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(sourcePath), path: sourcePath);
        foreach (
            var declaration in tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
        )
        {
            if (
                declaration.BaseList?.Types.Any(baseType =>
                    baseType.Type.ToString().Split('.').Last() == "NewKunlunCard"
                ) != true
            )
                continue;

            string relativePath = Path.GetRelativePath(linkRoot, sourcePath).Replace('\\', '/');
            cards[declaration.Identifier.ValueText] = new SourceCardInfo(
                relativePath,
                ReadCanonicalKeywords(declaration)
            );
        }
    }

    return cards;
}

static IReadOnlyList<string> ReadCanonicalKeywords(ClassDeclarationSyntax declaration)
{
    var property = declaration
        .Members.OfType<PropertyDeclarationSyntax>()
        .FirstOrDefault(property => property.Identifier.ValueText == "CanonicalKeywords");
    if (property is null)
        return [];

    SyntaxNode valueNode =
        property.ExpressionBody?.Expression ?? (SyntaxNode?)property.AccessorList ?? property;
    return valueNode
        .DescendantNodesAndSelf()
        .OfType<MemberAccessExpressionSyntax>()
        .Where(member =>
            member.Expression.ToString().EndsWith("CardKeyword", StringComparison.Ordinal)
            || member.Expression.ToString().EndsWith("Keywords", StringComparison.Ordinal)
        )
        .Select(member => SplitPascalCase(member.Name.Identifier.ValueText))
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToArray();
}

static SmartFormatter CreateGameSmartFormatter(Assembly projectAssembly)
{
    var formatter = Smart.CreateDefaultSmartFormat();
    var formatterTypes = new[]
    {
        projectAssembly,
        typeof(MegaCrit.Sts2.Core.Localization.Formatters.HighlightDifferencesFormatter).Assembly,
    }
        .Distinct()
        .SelectMany(GetLoadableTypes)
        .Where(type =>
            !type.IsAbstract
            && !type.ContainsGenericParameters
            && typeof(IFormatter).IsAssignableFrom(type)
            && type.FullName != "MegaCrit.Sts2.Core.Localization.Formatters.EnergyIconsFormatter"
            && !type.Name.Contains("CardNameVarFormatter", StringComparison.Ordinal)
            && type.GetConstructor(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                binder: null,
                Type.EmptyTypes,
                modifiers: null
            )
                is not null
        );

    var customFormatters = new List<IFormatter>
    {
        new MarkdownEnergyIconsFormatter(),
        new HeadlessCardNameFormatter(),
    };
    foreach (Type formatterType in formatterTypes)
    {
        try
        {
            if (Activator.CreateInstance(formatterType, nonPublic: true) is IFormatter extension)
                customFormatters.Add(extension);
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(
                $"Warning: could not load SmartFormat formatter {formatterType.FullName}: "
                    + exception.GetBaseException().Message
            );
        }
    }

    if (customFormatters.Count > 0)
        formatter.AddExtensions(customFormatters.ToArray());
    return formatter;
}

static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
{
    try
    {
        return assembly.GetTypes();
    }
    catch (ReflectionTypeLoadException exception)
    {
        return exception.Types.OfType<Type>();
    }
}

static IEnumerable<CardInfo> ReadCards(
    Assembly projectAssembly,
    IReadOnlyDictionary<string, SourceCardInfo> sourceCards,
    SmartFormatter smartFormatter
)
{
    foreach (
        Type cardType in projectAssembly
            .GetTypes()
            .Where(type => !type.IsAbstract && HasBaseType(type, "NewKunlunCard"))
    )
    {
        object? localization = cardType
            .GetCustomAttributes(inherit: false)
            .FirstOrDefault(attribute => attribute.GetType().Name == "CardLocalizationAttribute");
        if (localization is null || !sourceCards.TryGetValue(cardType.Name, out var sourceCard))
            continue;

        string title =
            ReadProperty(localization, "Title") as string
            ?? throw new InvalidOperationException(
                $"{cardType.FullName} has no localization title."
            );
        string rawDescription =
            ReadProperty(localization, "Description") as string
            ?? throw new InvalidOperationException(
                $"{cardType.FullName} has no localization description."
            );

        object card;
        try
        {
            card =
                Activator.CreateInstance(cardType)
                ?? throw new InvalidOperationException("The constructor returned null.");
            FindField(cardType, "<IsMutable>k__BackingField").SetValue(card, true);
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException(
                $"Could not construct {cardType.FullName} from the project build.",
                exception.GetBaseException()
            );
        }

        string type = ReadProperty(card, "Type")?.ToString() ?? "Unknown";
        string rarity = ReadProperty(card, "Rarity")?.ToString() ?? "Unknown";
        int maximumUpgradeLevel = Convert.ToInt32(ReadProperty(card, "MaxUpgradeLevel"));
        var renderStates = new List<CardRenderState>(maximumUpgradeLevel + 1);
        for (int upgradeLevel = 0; upgradeLevel <= maximumUpgradeLevel; upgradeLevel++)
        {
            renderStates.Add(
                new CardRenderState(
                    upgradeLevel,
                    ReadEnergyCost(card),
                    RenderDescription(card, rawDescription, smartFormatter),
                    ReadEffectiveKeywords(card, sourceCard.Keywords)
                )
            );
            if (upgradeLevel < maximumUpgradeLevel)
                InvokeUpgrade(card);
        }

        yield return new CardInfo(title, type, rarity, sourceCard.RelativePath, renderStates);
    }
}

static string RenderDescription(object card, string format, SmartFormatter smartFormatter)
{
    var formatArguments = ReadDynamicVars(card);
    format = ReplaceLiteralSelectors(format, formatArguments);
    try
    {
        return smartFormatter.Format(CultureInfo.GetCultureInfo("en-US"), format, formatArguments);
    }
    catch (Exception exception)
    {
        throw new InvalidOperationException(
            $"Could not render the description for {card.GetType().FullName}: {format}",
            exception.GetBaseException()
        );
    }
}

static void InvokeUpgrade(object card) =>
    card.GetType()
        .GetMethod(
            "UpgradeInternal",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
        )!
        .Invoke(card, null);

static string ReadEnergyCost(object card)
{
    if (ReadProperty(card, "HasEnergyCostX") is true)
        return "X";

    object? energyCost = ReadProperty(card, "EnergyCost");
    object? currentCost = energyCost is null
        ? null
        : FindField(energyCost.GetType(), "_base").GetValue(energyCost);
    return Convert.ToString(
            currentCost ?? ReadProperty(card, "CanonicalEnergyCost"),
            CultureInfo.InvariantCulture
        ) ?? "?";
}

static IReadOnlyList<string> ReadEffectiveKeywords(
    object card,
    IReadOnlyList<string> sourceKeywords
)
{
    var builtInNames = Enum.GetNames<MegaCrit.Sts2.Core.Entities.Cards.CardKeyword>()
        .ToHashSet(StringComparer.OrdinalIgnoreCase);
    var result = sourceKeywords
        .Where(keyword => !builtInNames.Contains(keyword))
        .ToHashSet(StringComparer.OrdinalIgnoreCase);

    if (ReadProperty(card, "Keywords") is IEnumerable runtimeKeywords)
        foreach (object keyword in runtimeKeywords)
        {
            string name = keyword.ToString()?.Split('.').Last() ?? "";
            if (name != "None" && builtInNames.Contains(name))
                result.Add(SplitPascalCase(name));
        }

    return result.ToArray();
}

static bool HasBaseType(Type type, string baseTypeName)
{
    for (Type? current = type.BaseType; current is not null; current = current.BaseType)
        if (current.Name == baseTypeName)
            return true;
    return false;
}

static string ReplaceLiteralSelectors(string format, IDictionary<string, object> arguments) =>
    Regex.Replace(
        Regex.Replace(format, @"(:cond:[^{}?]*\?[^{}|]*)(})", "$1|$2"),
        @"\{(?<number>-?\d+(?:\.\d+)?):",
        match =>
        {
            string key =
                $"Literal_{match.Groups["number"].Value.Replace('-', 'N').Replace('.', '_')}";
            arguments[key] = decimal.Parse(
                match.Groups["number"].Value,
                CultureInfo.InvariantCulture
            );
            return $"{{{key}:";
        }
    );

static object? ReadProperty(object instance, string propertyName) =>
    instance
        .GetType()
        .GetProperty(
            propertyName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
        )
        ?.GetValue(instance);

static FieldInfo FindField(Type type, string fieldName)
{
    for (Type? current = type; current is not null; current = current.BaseType)
        if (
            current.GetField(
                fieldName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
            ) is
            { } field
        )
            return field;

    throw new MissingFieldException(type.FullName, fieldName);
}

static Dictionary<string, object> ReadDynamicVars(object card)
{
    object dynamicVarSet =
        ReadProperty(card, "DynamicVars")
        ?? throw new InvalidOperationException($"{card.GetType().FullName} has no DynamicVars.");
    if (ReadProperty(dynamicVarSet, "Values") is not IEnumerable values)
        throw new InvalidOperationException(
            $"{card.GetType().FullName}.DynamicVars has no Values."
        );

    var result = new Dictionary<string, object>(StringComparer.Ordinal);
    foreach (object dynamicVar in values)
    {
        string? name = ReadProperty(dynamicVar, "Name") as string;
        if (!string.IsNullOrWhiteSpace(name))
            result[name] = dynamicVar;
    }
    result["IfUpgraded"] = new MegaCrit.Sts2.Core.Localization.DynamicVars.IfUpgradedVar(
        Convert.ToInt32(ReadProperty(card, "CurrentUpgradeLevel")) > 0
            ? MegaCrit.Sts2.Core.Localization.UpgradeDisplay.Upgraded
            : MegaCrit.Sts2.Core.Localization.UpgradeDisplay.Normal
    );
    return result;
}

static string RenderMarkdown(IReadOnlyCollection<CardInfo> cards)
{
    string[] standardRarities = ["Common", "Uncommon", "Rare"];
    string[] typeOrder = ["Attack", "Skill", "Power"];
    var builder = new StringBuilder();

    builder.AppendLine("# New Kunlun Cards");
    builder.AppendLine();
    builder.AppendLine(
        "Generated from the compiled card implementations. Do not edit this file manually."
    );
    builder.AppendLine();
    builder.AppendLine(
        "<style>.nk-card-description{display:block;margin-left:1.5em}.nk-upgrade-state[hidden]{display:none}.nk-upgrade-button{margin-right:.35em;padding:0 .45em}.nk-energy-cost{white-space:nowrap}</style>"
    );

    foreach (string rarity in standardRarities)
        AppendRaritySection(builder, rarity, cards.Where(card => card.Rarity == rarity), typeOrder);

    foreach (
        var rarityGroup in cards
            .Where(card => !standardRarities.Contains(card.Rarity, StringComparer.Ordinal))
            .GroupBy(card => card.Rarity)
            .OrderBy(group => UniqueRarityOrder(group.Key))
            .ThenBy(group => group.Key, StringComparer.OrdinalIgnoreCase)
    )
        AppendRaritySection(builder, DisplayRarity(rarityGroup.Key), rarityGroup, typeOrder);

    AppendCounts(builder, cards, standardRarities, typeOrder);
    return builder.ToString();
}

static void AppendRaritySection(
    StringBuilder builder,
    string heading,
    IEnumerable<CardInfo> sectionCards,
    IReadOnlyList<string> typeOrder
)
{
    var materialized = sectionCards.ToArray();
    if (materialized.Length == 0)
        return;

    builder.AppendLine();
    builder.AppendLine($"## {heading}");

    foreach (
        var typeGroup in materialized
            .GroupBy(card => card.Type)
            .OrderBy(group => TypeOrder(group.Key, typeOrder))
            .ThenBy(group => group.Key, StringComparer.OrdinalIgnoreCase)
    )
    {
        builder.AppendLine();
        builder.AppendLine($"### {typeGroup.Key}");
        builder.AppendLine();
        foreach (
            var card in typeGroup.OrderBy(card => card.Title, StringComparer.OrdinalIgnoreCase)
        )
        {
            builder
                .Append("- <span class=\"nk-card\" data-upgrade=\"0\" data-levels=\"")
                .Append(card.RenderStates.Count)
                .Append("\">");
            if (card.RenderStates.Count > 1)
                builder.Append(
                    "<button class=\"nk-upgrade-button\" type=\"button\" title=\"Show upgrade level 1\" onclick=\"const card=this.closest('.nk-card');const levels=Number(card.dataset.levels);const next=(Number(card.dataset.upgrade)+1)%levels;const following=(next+1)%levels;card.dataset.upgrade=next;card.querySelector('.nk-title-upgrades').textContent=next===0?'':levels===2?'+':`+${next}`;card.querySelectorAll('.nk-upgrade-state').forEach(state=>state.hidden=Number(state.dataset.upgrade)!==next);this.textContent=following===0?'-':'+';this.title=following===0?'Return to upgrade level 0':`Show upgrade level ${following}`\">+</button>"
                );
            builder
                .Append("<a href=\"")
                .Append(card.RelativePath.Replace(" ", "%20"))
                .Append("\">")
                .Append(EscapeHtml(card.Title))
                .Append("<span class=\"nk-title-upgrades\"></span></a>");
            builder.Append(" — ");
            foreach (var state in card.RenderStates)
            {
                string description = ConvertGameMarkupToMarkdown(
                        AddCanonicalKeywords(state.Description, state.Keywords)
                    )
                    .Replace("\n", "<br>");
                builder
                    .Append("<span class=\"nk-upgrade-state nk-energy-cost\" data-upgrade=\"")
                    .Append(state.UpgradeLevel)
                    .Append('"');
                if (state.UpgradeLevel > 0)
                    builder.Append(" hidden");
                builder
                    .Append('>')
                    .Append(RenderEnergyCost(card.RenderStates[0], state))
                    .Append("</span>")
                    .Append("<span class=\"nk-upgrade-state nk-card-description\" data-upgrade=\"")
                    .Append(state.UpgradeLevel)
                    .Append('"');
                if (state.UpgradeLevel > 0)
                    builder.Append(" hidden");
                builder.Append('>').Append(description).Append("</span>");
            }
            builder.AppendLine("</span>");
        }
    }
}

static string RenderEnergyCost(CardRenderState baseState, CardRenderState state)
{
    string renderedCost = $"{EscapeHtml(state.EnergyCost)} ⚡";
    bool isReduced =
        state.UpgradeLevel > 0
        && decimal.TryParse(
            baseState.EnergyCost,
            NumberStyles.Number,
            CultureInfo.InvariantCulture,
            out decimal baseCost
        )
        && decimal.TryParse(
            state.EnergyCost,
            NumberStyles.Number,
            CultureInfo.InvariantCulture,
            out decimal currentCost
        )
        && currentCost < baseCost;
    return isReduced ? $"<strong style=\"color:#52cb84\">{renderedCost}</strong>" : renderedCost;
}

static string AddCanonicalKeywords(string description, IReadOnlyList<string> keywords)
{
    var missing = keywords
        .Where(keyword => !DescriptionContainsKeyword(description, keyword))
        .ToArray();
    var leading = missing.Where(IsLeadingKeyword).Select(HighlightKeyword).ToArray();
    var trailing = missing
        .Where(keyword => !IsLeadingKeyword(keyword))
        .Select(HighlightKeyword)
        .ToArray();

    string result = description.Trim();
    if (leading.Length > 0)
        result = $"{string.Join(". ", leading)}. {result}";
    if (trailing.Length > 0)
        result = $"{result.TrimEnd()} {string.Join(". ", trailing)}.";
    return result;
}

static string ConvertGameMarkupToMarkdown(string value)
{
    value = Regex.Replace(
        value,
        @"\[gold\](.*?)\[/gold\]",
        "<strong style=\"color:#e7ae33\">$1</strong>",
        RegexOptions.Singleline | RegexOptions.IgnoreCase
    );
    value = Regex.Replace(
        value,
        @"\[green\](.*?)\[/green\]",
        "<strong style=\"color:#52cb84\">$1</strong>",
        RegexOptions.Singleline | RegexOptions.IgnoreCase
    );
    value = Regex.Replace(
        value,
        @"\[red\](.*?)\[/red\]",
        "<span style=\"color:#e05252\">$1</span>",
        RegexOptions.Singleline | RegexOptions.IgnoreCase
    );
    value = Regex.Replace(value, @"\[img\][^\[]*energy_icon\.png\[/img\]", "⚡");
    value = Regex.Replace(value, @"\[/?(?:gold|green|red)\]", "", RegexOptions.IgnoreCase);
    return value;
}

static bool DescriptionContainsKeyword(string description, string keyword)
{
    string normalizedDescription = Regex.Replace(description, @"\[/?(?:gold|green|red)\]", "");
    return normalizedDescription.Contains(keyword, StringComparison.OrdinalIgnoreCase);
}

static bool IsLeadingKeyword(string keyword) =>
    keyword is "Innate" or "Ethereal" or "Retain" or "Unplayable";

static string HighlightKeyword(string keyword) => $"[gold]{keyword}[/gold]";

static void AppendCounts(
    StringBuilder builder,
    IReadOnlyCollection<CardInfo> cards,
    IReadOnlyList<string> rarities,
    IReadOnlyList<string> types
)
{
    builder.AppendLine();
    builder.AppendLine("## Card Counts");
    builder.AppendLine();
    builder.AppendLine("| Rarity | Attack | Skill | Power | Total |");
    builder.AppendLine("| --- | ---: | ---: | ---: | ---: |");
    foreach (string rarity in rarities)
    {
        builder.Append("| ").Append(rarity);
        foreach (string type in types)
            builder
                .Append(" | ")
                .Append(cards.Count(card => card.Rarity == rarity && card.Type == type));
        builder
            .Append(" | ")
            .Append(cards.Count(card => card.Rarity == rarity && types.Contains(card.Type)));
        builder.AppendLine(" |");
    }

    builder.Append("| **Total**");
    foreach (string type in types)
        builder
            .Append(" | **")
            .Append(cards.Count(card => rarities.Contains(card.Rarity) && card.Type == type))
            .Append("**");
    builder
        .Append(" | **")
        .Append(cards.Count(card => rarities.Contains(card.Rarity) && types.Contains(card.Type)))
        .AppendLine("** |");
}

static int TypeOrder(string type, IReadOnlyList<string> typeOrder)
{
    for (int index = 0; index < typeOrder.Count; index++)
        if (typeOrder[index] == type)
            return index;
    return int.MaxValue;
}

static int UniqueRarityOrder(string rarity) =>
    rarity switch
    {
        "Basic" => 0,
        "Status" => 1,
        _ => 2,
    };

static string DisplayRarity(string rarity) => rarity == "Basic" ? "Starter" : rarity;

static string SplitPascalCase(string value)
{
    var builder = new StringBuilder(value.Length + 4);
    for (int index = 0; index < value.Length; index++)
    {
        if (index > 0 && char.IsUpper(value[index]) && char.IsLower(value[index - 1]))
            builder.Append(' ');
        builder.Append(value[index]);
    }
    return builder.ToString();
}

static string EscapeHtml(string value) =>
    value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");

internal sealed record CardInfo(
    string Title,
    string Type,
    string Rarity,
    string RelativePath,
    IReadOnlyList<CardRenderState> RenderStates
);

internal sealed record CardRenderState(
    int UpgradeLevel,
    string EnergyCost,
    string Description,
    IReadOnlyList<string> Keywords
);

internal sealed record SourceCardInfo(string RelativePath, IReadOnlyList<string> Keywords);

internal sealed class MarkdownEnergyIconsFormatter : IFormatter
{
    public string Name
    {
        get => "energyIcons";
        set => throw new InvalidOperationException();
    }

    public bool CanAutoDetect { get; set; }

    public bool TryEvaluateFormat(IFormattingInfo formattingInfo)
    {
        if (formattingInfo.CurrentValue is not IConvertible value)
            return false;

        int amount = Convert.ToInt32(value, formattingInfo.FormatDetails.Provider);
        formattingInfo.Write(amount is > 0 and < 4 ? new string('⚡', amount) : $"{amount}⚡");
        return true;
    }
}

internal sealed class HeadlessCardNameFormatter : IFormatter
{
    public string Name
    {
        get => "cardName";
        set => throw new InvalidOperationException();
    }

    public bool CanAutoDetect { get; set; }

    public bool TryEvaluateFormat(IFormattingInfo formattingInfo)
    {
        object? value = formattingInfo.CurrentValue;
        if (value is null || !IsCardNameVar(value.GetType()))
            return false;

        Type type = value.GetType();
        string title =
            type.GetField("Title", BindingFlags.NonPublic | BindingFlags.Static)!
                .GetValue(null)
                ?.ToString()
            ?? "???";
        var upgradePredicate = type.GetFields(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
            )
            .Select(field => field.GetValue(value))
            .OfType<Func<bool>>()
            .FirstOrDefault();
        bool upgraded = upgradePredicate?.Invoke() == true;

        formattingInfo.Write(upgraded ? $"[green]{title}+[/green]" : $"[gold]{title}[/gold]");
        return true;
    }

    private static bool IsCardNameVar(Type type)
    {
        for (Type? current = type; current is not null; current = current.BaseType)
            if (current.IsGenericType && current.GetGenericTypeDefinition().Name == "CardNameVar`1")
                return true;
        return false;
    }
}
