using System.Text;
using System.Text.RegularExpressions;
using LocalizationTool;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;

if (!TryReadArguments(args, out var options))
{
    Console.Error.WriteLine(
        "Usage: LocalizationTool --source <directory> --localization-dir <directory> "
            + "--cards <directory> --cards-json <cards.json> "
            + "--afflictions <directory> --afflictions-json <afflictions.json> "
            + "--card-modifiers <directory> --card-modifiers-json <card_modifiers.json> "
            + "--powers <directory> --powers-json <powers.json> "
            + "--enchantments <directory> --enchantments-json <enchantments.json> "
            + "--potions <directory> --potions-json <potions.json> "
            + "--relics <directory> --relics-json <relics.json> "
            + "--keywords <directory> --keywords-json <card_keywords.json> "
            + "--static-hover-tips <directory> --static-hover-tips-json <static_hover_tips.json> "
            + "--prefix <localization prefix>"
    );
    return 2;
}

Dictionary<string, SortedDictionary<string, string>> sheets = new(StringComparer.OrdinalIgnoreCase);
Dictionary<string, string> sheetPaths = new(StringComparer.OrdinalIgnoreCase);

void ContributeModels(
    string sourceDirectory,
    string baseTypeName,
    string attributeName,
    string jsonPath,
    string modelKind
)
{
    var models = ReadModels(sourceDirectory, baseTypeName, attributeName);
    var entries = GetSheet(jsonPath, preserveExisting: false);
    foreach (var model in models)
    {
        var modelId = $"{options.IdPrefix}-{ToUpperSnakeCase(model.ClassName)}";
        foreach (var value in model.Values)
            entries[$"{modelId}.{value.Name}"] = value.Value;
    }
    Console.WriteLine($"Collected {models.Count} {modelKind} annotation(s) for {jsonPath}.");
}

SortedDictionary<string, string> GetSheet(string jsonPath, bool preserveExisting)
{
    var sheetName = Path.GetFileNameWithoutExtension(jsonPath);
    if (sheets.TryGetValue(sheetName, out var existing))
        return existing;

    var entries =
        preserveExisting && File.Exists(jsonPath)
            ? JsonConvert.DeserializeObject<SortedDictionary<string, string>>(
                File.ReadAllText(jsonPath)
            ) ?? []
            : [];
    sheets.Add(sheetName, entries);
    sheetPaths.Add(sheetName, jsonPath);
    return entries;
}

ContributeModels(
    options.AfflictionsDirectory,
    "AfflictionModel",
    "AfflictionLocalization",
    options.AfflictionsJsonPath,
    "affliction"
);
ContributeModels(
    options.CardsDirectory,
    "NewKunlunCard",
    "CardLocalization",
    options.CardsJsonPath,
    "card"
);
ContributeModels(
    options.CardModifiersDirectory,
    "NewKunlunModifier",
    "CardModifierLocalization",
    options.CardModifiersJsonPath,
    "card modifier"
);
ContributeModels(
    options.EnchantmentsDirectory,
    "NewKunlunEnchantment",
    "EnchantmentLocalization",
    options.EnchantmentsJsonPath,
    "enchantment"
);
ContributeModels(
    options.PowersDirectory,
    "NewKunlunPower",
    "PowerLocalization",
    options.PowersJsonPath,
    "power"
);
ContributeModels(
    options.PotionsDirectory,
    "NewKunlunPotion",
    "PotionLocalization",
    options.PotionsJsonPath,
    "potion"
);
ContributeModels(
    options.RelicsDirectory,
    "NewKunlunRelic",
    "RelicLocalization",
    options.RelicsJsonPath,
    "relic"
);

var keywords = ReadLocalizedFields(
    options.KeywordsDirectory,
    "CardKeyword",
    "KeywordLocalization",
    requireCustomEnum: false
);
ContributeFields(
    GetSheet(options.KeywordsJsonPath, preserveExisting: true),
    options.IdPrefix,
    keywords
);
Console.WriteLine(
    $"Collected {keywords.Count} keyword annotation(s) for {options.KeywordsJsonPath}."
);

var staticHoverTips = ReadLocalizedFields(
    options.StaticHoverTipsDirectory,
    "StaticHoverTip",
    "StaticHoverTipLocalization",
    requireCustomEnum: true
);
ContributeFields(
    GetSheet(options.StaticHoverTipsJsonPath, preserveExisting: true),
    options.IdPrefix,
    staticHoverTips
);
Console.WriteLine(
    $"Collected {staticHoverTips.Count} static hover tip annotation(s) for {options.StaticHoverTipsJsonPath}."
);

var customLocStrings = ReadCustomLocStrings(options.SourceDirectory);
foreach (var definition in customLocStrings)
{
    var jsonPath = Path.Combine(options.LocalizationDirectory, $"{definition.TableName}.json");
    GetSheet(jsonPath, preserveExisting: true)[definition.Key] = definition.Description;
}
Console.WriteLine($"Collected {customLocStrings.Count} custom LocString annotation(s).");

foreach (var sheetName in sheets.Keys.OrderBy(name => name, StringComparer.Ordinal))
{
    var jsonPath = sheetPaths[sheetName];
    var json = JsonConvert
        .SerializeObject(sheets[sheetName], Formatting.Indented)
        .Replace("\r\n", "\n");
    Directory.CreateDirectory(Path.GetDirectoryName(jsonPath)!);
    File.WriteAllText(jsonPath, json, new UTF8Encoding(false));
    Console.WriteLine($"Updated {jsonPath} with {sheets[sheetName].Count} entries.");
}
return 0;

static IReadOnlyList<CustomLocStringDefinition> ReadCustomLocStrings(string sourceDirectory)
{
    List<CustomLocStringDefinition> definitions = [];
    foreach (
        var sourcePath in Directory.EnumerateFiles(
            sourceDirectory,
            "*.cs",
            SearchOption.AllDirectories
        )
    )
    {
        var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(sourcePath), path: sourcePath);
        foreach (
            var property in tree.GetRoot().DescendantNodes().OfType<PropertyDeclarationSyntax>()
        )
        {
            var attribute = property
                .AttributeLists.SelectMany(list => list.Attributes)
                .FirstOrDefault(candidate => IsLocalizationAttribute(candidate, "CustomLocString"));
            if (
                attribute?.ArgumentList is not { } arguments
                || property.Parent is not ClassDeclarationSyntax clazz
            )
                continue;

            var tableArgument = arguments.Arguments.FirstOrDefault(argument =>
                argument.NameColon?.Name.Identifier.ValueText is null or "table"
            );
            var descriptionArgument = arguments.Arguments.FirstOrDefault(argument =>
                argument.NameColon?.Name.Identifier.ValueText == "description"
            );
            if (descriptionArgument is null && arguments.Arguments.Count > 1)
                descriptionArgument = arguments.Arguments[1];

            var tableMemberName = tableArgument?.Expression switch
            {
                MemberAccessExpressionSyntax member => member.Name.Identifier.ValueText,
                IdentifierNameSyntax identifier => identifier.Identifier.ValueText,
                _ => null,
            };
            if (
                tableMemberName is null
                || descriptionArgument?.Expression is not LiteralExpressionSyntax description
                || !description.IsKind(SyntaxKind.StringLiteralExpression)
            )
                continue;

            definitions.Add(
                new CustomLocStringDefinition(
                    ToLowerSnakeCase(tableMemberName),
                    $"NEW_KUNLUN-{ToUpperSnakeCase(clazz.Identifier.ValueText)}-{ToUpperSnakeCase(property.Identifier.ValueText)}",
                    description.Token.ValueText
                )
            );
        }
    }
    return definitions;
}

static void ContributeFields(
    SortedDictionary<string, string> entries,
    string idPrefix,
    IReadOnlyList<LocalizedModel> fields
)
{
    foreach (var field in fields)
    {
        var fieldId = $"{idPrefix}-{field.ClassName.ToUpperInvariant()}";
        foreach (var value in field.Values)
            entries[$"{fieldId}.{value.Name}"] = value.Value;
    }
}

static IReadOnlyList<LocalizedModel> ReadLocalizedFields(
    string sourceDirectory,
    string fieldTypeName,
    string attributeName,
    bool requireCustomEnum
)
{
    List<LocalizedModel> fields = [];
    foreach (
        var sourcePath in Directory.EnumerateFiles(
            sourceDirectory,
            "*.cs",
            SearchOption.AllDirectories
        )
    )
    {
        var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(sourcePath), path: sourcePath);
        foreach (var field in tree.GetRoot().DescendantNodes().OfType<FieldDeclarationSyntax>())
        {
            if (
                !field.Modifiers.Any(SyntaxKind.StaticKeyword)
                || field.Declaration.Type.ToString().Split('.').Last() != fieldTypeName
                || (
                    requireCustomEnum
                    && !field
                        .AttributeLists.SelectMany(list => list.Attributes)
                        .Any(attribute => IsLocalizationAttribute(attribute, "CustomEnum"))
                )
            )
                continue;

            var attribute = field
                .AttributeLists.SelectMany(list => list.Attributes)
                .FirstOrDefault(candidate => IsLocalizationAttribute(candidate, attributeName));
            if (attribute?.ArgumentList is not { } arguments)
                continue;

            var values = ReadLocalizationValues(arguments);
            foreach (var variable in field.Declaration.Variables)
                fields.Add(new LocalizedModel(variable.Identifier.ValueText, values));
        }
    }
    return fields;
}

static IReadOnlyList<LocalizedModel> ReadModels(
    string sourceDirectory,
    string baseTypeName,
    string attributeName
)
{
    List<LocalizedModel> models = [];
    foreach (
        var sourcePath in Directory.EnumerateFiles(
            sourceDirectory,
            "*.cs",
            SearchOption.AllDirectories
        )
    )
    {
        var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(sourcePath), path: sourcePath);
        foreach (var clazz in tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>())
        {
            if (!Extends(clazz, baseTypeName))
                continue;

            var attribute = FindLocalizationAttribute(clazz, attributeName);
            if (attribute?.ArgumentList is not { } arguments)
                continue;

            models.Add(
                new LocalizedModel(clazz.Identifier.ValueText, ReadLocalizationValues(arguments))
            );
        }
    }
    return models;
}

static IReadOnlyList<LocalizationValue> ReadLocalizationValues(
    AttributeArgumentListSyntax arguments
)
{
    var values = new List<LocalizationValue>(arguments.Arguments.Count);
    foreach (var argument in arguments.Arguments)
    {
        var name = argument.NameColon?.Name.Identifier.ValueText;
        if (
            name is not null
            && argument.Expression is LiteralExpressionSyntax literal
            && literal.IsKind(SyntaxKind.StringLiteralExpression)
        )
            values.Add(new LocalizationValue(name, literal.Token.ValueText));
    }
    return values;
}

static bool TryReadArguments(string[] arguments, out ToolOptions options)
{
    var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    for (var index = 0; index + 1 < arguments.Length; index += 2)
        values[arguments[index]] = arguments[index + 1];

    options = new ToolOptions(
        values.GetValueOrDefault("--source") ?? "",
        values.GetValueOrDefault("--localization-dir") ?? "",
        values.GetValueOrDefault("--afflictions") ?? "",
        values.GetValueOrDefault("--afflictions-json") ?? "",
        values.GetValueOrDefault("--cards") ?? "",
        values.GetValueOrDefault("--cards-json") ?? "",
        values.GetValueOrDefault("--card-modifiers") ?? "",
        values.GetValueOrDefault("--card-modifiers-json") ?? "",
        values.GetValueOrDefault("--enchantments") ?? "",
        values.GetValueOrDefault("--enchantments-json") ?? "",
        values.GetValueOrDefault("--powers") ?? "",
        values.GetValueOrDefault("--powers-json") ?? "",
        values.GetValueOrDefault("--potions") ?? "",
        values.GetValueOrDefault("--potions-json") ?? "",
        values.GetValueOrDefault("--relics") ?? "",
        values.GetValueOrDefault("--relics-json") ?? "",
        values.GetValueOrDefault("--keywords") ?? "",
        values.GetValueOrDefault("--keywords-json") ?? "",
        values.GetValueOrDefault("--static-hover-tips") ?? "",
        values.GetValueOrDefault("--static-hover-tips-json") ?? "",
        values.GetValueOrDefault("--prefix") ?? ""
    );
    return Directory.Exists(options.SourceDirectory)
        && Directory.Exists(options.LocalizationDirectory)
        && Directory.Exists(options.AfflictionsDirectory)
        && Directory.Exists(options.CardsDirectory)
        && Directory.Exists(options.CardModifiersDirectory)
        && Directory.Exists(options.PowersDirectory)
        && Directory.Exists(options.PotionsDirectory)
        && Directory.Exists(options.EnchantmentsDirectory)
        && Directory.Exists(options.RelicsDirectory)
        && Directory.Exists(options.KeywordsDirectory)
        && Directory.Exists(options.StaticHoverTipsDirectory)
        && !string.IsNullOrWhiteSpace(options.AfflictionsJsonPath)
        && !string.IsNullOrWhiteSpace(options.CardsJsonPath)
        && !string.IsNullOrWhiteSpace(options.CardModifiersJsonPath)
        && !string.IsNullOrWhiteSpace(options.PowersJsonPath)
        && !string.IsNullOrWhiteSpace(options.PotionsJsonPath)
        && !string.IsNullOrWhiteSpace(options.EnchantmentsJsonPath)
        && !string.IsNullOrWhiteSpace(options.RelicsJsonPath)
        && !string.IsNullOrWhiteSpace(options.KeywordsJsonPath)
        && !string.IsNullOrWhiteSpace(options.StaticHoverTipsJsonPath)
        && !string.IsNullOrWhiteSpace(options.IdPrefix);
}

static bool Extends(ClassDeclarationSyntax clazz, string baseTypeName) =>
    clazz.BaseList?.Types.Any(type => type.Type.ToString().Split('.').Last() == baseTypeName)
    == true;

static AttributeSyntax? FindLocalizationAttribute(
    ClassDeclarationSyntax clazz,
    string attributeName
) =>
    clazz
        .AttributeLists.SelectMany(list => list.Attributes)
        .FirstOrDefault(attribute => IsLocalizationAttribute(attribute, attributeName));

static bool IsLocalizationAttribute(AttributeSyntax attribute, string attributeName) =>
    attribute.Name.ToString() == attributeName
    || attribute.Name.ToString() == $"{attributeName}Attribute"
    || attribute.Name.ToString().EndsWith($".{attributeName}")
    || attribute.Name.ToString().EndsWith($".{attributeName}Attribute");

static string ToUpperSnakeCase(string value) =>
    Regex.Replace(value, "(?<=[a-z0-9])([A-Z])", "_$1").ToUpperInvariant();

static string ToLowerSnakeCase(string value) =>
    Regex.Replace(value, "(?<=[a-z0-9])([A-Z])", "_$1").ToLowerInvariant();

namespace LocalizationTool
{
    internal sealed record ToolOptions(
        string SourceDirectory,
        string LocalizationDirectory,
        string AfflictionsDirectory,
        string AfflictionsJsonPath,
        string CardsDirectory,
        string CardsJsonPath,
        string CardModifiersDirectory,
        string CardModifiersJsonPath,
        string EnchantmentsDirectory,
        string EnchantmentsJsonPath,
        string PowersDirectory,
        string PowersJsonPath,
        string PotionsDirectory,
        string PotionsJsonPath,
        string RelicsDirectory,
        string RelicsJsonPath,
        string KeywordsDirectory,
        string KeywordsJsonPath,
        string StaticHoverTipsDirectory,
        string StaticHoverTipsJsonPath,
        string IdPrefix
    );

    internal sealed record LocalizedModel(
        string ClassName,
        IReadOnlyList<LocalizationValue> Values
    );

    internal sealed record LocalizationValue(string Name, string Value);

    internal sealed record CustomLocStringDefinition(
        string TableName,
        string Key,
        string Description
    );
}
