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
        "Usage: LocalizationTool --cards <directory> --cards-json <cards.json> "
            + "--powers <directory> --powers-json <powers.json> "
            + "--relics <directory> --relics-json <relics.json> "
            + "--keywords <directory> --keywords-json <card_keywords.json> "
            + "--static-hover-tips <directory> --static-hover-tips-json <static_hover_tips.json> "
            + "--prefix <localization prefix>"
    );
    return 2;
}

var cards = ReadModels(options.CardsDirectory, "NewKunlunCard", "CardLocalization");
var powers = ReadModels(options.PowersDirectory, "NewKunlunPower", "PowerLocalization");
var relics = ReadModels(options.RelicsDirectory, "NewKunlunRelic", "RelicLocalization");
var keywords = ReadKeywords(options.KeywordsDirectory);
var staticHoverTips = ReadStaticHoverTips(options.StaticHoverTipsDirectory);

UpdateJson(options.CardsJsonPath, options.IdPrefix, cards, "card");
UpdateJson(options.PowersJsonPath, options.IdPrefix, powers, "power");
UpdateJson(options.RelicsJsonPath, options.IdPrefix, relics, "relic");
UpdateKeywordJson(options.KeywordsJsonPath, options.IdPrefix, keywords);
UpdateFieldJson(
    options.StaticHoverTipsJsonPath,
    options.IdPrefix,
    staticHoverTips,
    "static hover tip"
);
return 0;

static IReadOnlyList<LocalizedModel> ReadStaticHoverTips(string sourceDirectory) =>
    ReadLocalizedFields(
        sourceDirectory,
        "StaticHoverTip",
        "StaticHoverTipLocalization",
        requireCustomEnum: true
    );

static IReadOnlyList<LocalizedModel> ReadKeywords(string sourceDirectory) =>
    ReadLocalizedFields(sourceDirectory, "CardKeyword", "KeywordLocalization", false);

static IReadOnlyList<LocalizedModel> ReadLocalizedFields(
    string sourceDirectory,
    string fieldTypeName,
    string attributeName,
    bool requireCustomEnum
)
{
    List<LocalizedModel> keywords = [];
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
                keywords.Add(new LocalizedModel(variable.Identifier.ValueText, values));
        }
    }

    return keywords;
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

            var values = ReadLocalizationValues(arguments);

            models.Add(new LocalizedModel(clazz.Identifier.ValueText, values));
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

static void UpdateJson(
    string jsonPath,
    string idPrefix,
    IReadOnlyList<LocalizedModel> models,
    string modelKind
)
{
    SortedDictionary<string, string> entries = [];
    foreach (var model in models)
    {
        var modelId = $"{idPrefix}-{ToUpperSnakeCase(model.ClassName)}";
        foreach (var value in model.Values)
            entries[$"{modelId}.{value.Name}"] = value.Value;
    }

    var json = JsonConvert.SerializeObject(entries, Formatting.Indented).Replace("\r\n", "\n");

    Directory.CreateDirectory(Path.GetDirectoryName(jsonPath)!);
    File.WriteAllText(jsonPath, json, new UTF8Encoding(false));
    Console.WriteLine($"Updated {jsonPath} from {models.Count} {modelKind} annotation(s).");
}

static void UpdateKeywordJson(
    string jsonPath,
    string idPrefix,
    IReadOnlyList<LocalizedModel> keywords
)
{
    var entries = File.Exists(jsonPath)
        ? JsonConvert.DeserializeObject<SortedDictionary<string, string>>(
            File.ReadAllText(jsonPath)
        ) ?? []
        : new SortedDictionary<string, string>();

    foreach (var keyword in keywords)
    {
        var keywordId = $"{idPrefix}-{keyword.ClassName.ToUpperInvariant()}";
        foreach (var value in keyword.Values)
            entries[$"{keywordId}.{value.Name}"] = value.Value;
    }

    var json = JsonConvert.SerializeObject(entries, Formatting.Indented).Replace("\r\n", "\n");
    Directory.CreateDirectory(Path.GetDirectoryName(jsonPath)!);
    File.WriteAllText(jsonPath, json, new UTF8Encoding(false));
    Console.WriteLine($"Updated {jsonPath} from {keywords.Count} keyword annotation(s).");
}

static void UpdateFieldJson(
    string jsonPath,
    string idPrefix,
    IReadOnlyList<LocalizedModel> fields,
    string fieldKind
)
{
    var entries = File.Exists(jsonPath)
        ? JsonConvert.DeserializeObject<SortedDictionary<string, string>>(
            File.ReadAllText(jsonPath)
        ) ?? []
        : new SortedDictionary<string, string>();

    foreach (var field in fields)
    {
        var fieldId = $"{idPrefix}-{field.ClassName.ToUpperInvariant()}";
        foreach (var value in field.Values)
            entries[$"{fieldId}.{value.Name}"] = value.Value;
    }

    var json = JsonConvert.SerializeObject(entries, Formatting.Indented).Replace("\r\n", "\n");
    Directory.CreateDirectory(Path.GetDirectoryName(jsonPath)!);
    File.WriteAllText(jsonPath, json, new UTF8Encoding(false));
    Console.WriteLine($"Updated {jsonPath} from {fields.Count} {fieldKind} annotation(s).");
}

static bool TryReadArguments(string[] arguments, out ToolOptions options)
{
    var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    for (var index = 0; index + 1 < arguments.Length; index += 2)
        values[arguments[index]] = arguments[index + 1];

    options = new ToolOptions(
        values.GetValueOrDefault("--cards") ?? "",
        values.GetValueOrDefault("--cards-json") ?? "",
        values.GetValueOrDefault("--powers") ?? "",
        values.GetValueOrDefault("--powers-json") ?? "",
        values.GetValueOrDefault("--relics") ?? "",
        values.GetValueOrDefault("--relics-json") ?? "",
        values.GetValueOrDefault("--keywords") ?? "",
        values.GetValueOrDefault("--keywords-json") ?? "",
        values.GetValueOrDefault("--static-hover-tips") ?? "",
        values.GetValueOrDefault("--static-hover-tips-json") ?? "",
        values.GetValueOrDefault("--prefix") ?? ""
    );
    return Directory.Exists(options.CardsDirectory)
        && Directory.Exists(options.PowersDirectory)
        && Directory.Exists(options.RelicsDirectory)
        && Directory.Exists(options.KeywordsDirectory)
        && Directory.Exists(options.StaticHoverTipsDirectory)
        && !string.IsNullOrWhiteSpace(options.CardsJsonPath)
        && !string.IsNullOrWhiteSpace(options.PowersJsonPath)
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

namespace LocalizationTool
{
    internal sealed record ToolOptions(
        string CardsDirectory,
        string CardsJsonPath,
        string PowersDirectory,
        string PowersJsonPath,
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
}
