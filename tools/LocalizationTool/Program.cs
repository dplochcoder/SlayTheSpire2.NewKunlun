using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

if (!TryReadArguments(args, out var options))
{
    Console.Error.WriteLine(
        "Usage: LocalizationTool --cards <directory> --cards-json <cards.json> "
            + "--powers <directory> --powers-json <powers.json> "
            + "--relics <directory> --relics-json <relics.json> --prefix <localization prefix>"
    );
    return 2;
}

var cards = ReadModels(options.CardsDirectory, "NewKunlunCard", "CardLocalization");
var powers = ReadModels(options.PowersDirectory, "NewKunlunPower", "PowerLocalization");
var relics = ReadModels(options.RelicsDirectory, "NewKunlunRelic", "RelicLocalization");

UpdateJson(options.CardsJsonPath, options.IdPrefix, cards, "card");
UpdateJson(options.PowersJsonPath, options.IdPrefix, powers, "power");
UpdateJson(options.RelicsJsonPath, options.IdPrefix, relics, "relic");
return 0;

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

            var attr = FindLocalizationAttribute(clazz, attributeName);
            if (attr?.ArgumentList is not { } argumentList)
                continue;

            var values = argumentList
                .Arguments.Select(
                    (argument, index) =>
                        new LocalizationValue(
                            argument.NameColon!.Name.Identifier.ValueText,
                            ((LiteralExpressionSyntax)argument.Expression).Token.ValueText
                        )
                )
                .ToArray();
            models.Add(new LocalizedModel(clazz.Identifier.ValueText, values));
        }
    }
    return models;
}

static void UpdateJson(
    string jsonPath,
    string idPrefix,
    IReadOnlyList<LocalizedModel> models,
    string modelKind
)
{
    SortedDictionary<string, string> generatedJson = [];
    foreach (var model in models)
    {
        var modelId = $"{idPrefix}-{ToUpperSnakeCase(model.ClassName)}";
        foreach (var value in model.Values)
            generatedJson[$"{modelId}.{value.Name}"] = value.Value;
    }

    var existingText = File.Exists(jsonPath) ? File.ReadAllText(jsonPath) : null;
    SortedDictionary<string, string> existing = [];
    if (existingText is not null)
        existing = JsonSerializer.Deserialize<SortedDictionary<string, string>>(existingText) ?? [];

    foreach (var (key, value) in generatedJson)
        existing[key] = value;

    var newline = existingText?.Contains("\r\n") == true ? "\r\n" : "\n";
    var json =
        JsonSerializer
            .Serialize(existing, new JsonSerializerOptions { WriteIndented = true })
            .Replace("\r\n", "\n")
            .Replace("\n", newline) + newline;
    if (existingText != json)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(jsonPath)!);
        File.WriteAllText(jsonPath, json);
        Console.WriteLine($"Updated {jsonPath} from {models.Count} {modelKind} annotation(s).");
    }
    else
    {
        Console.WriteLine(
            $"{Path.GetFileName(jsonPath)} is current for {models.Count} {modelKind} annotation(s)."
        );
    }
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
        values.GetValueOrDefault("--prefix") ?? ""
    );
    return Directory.Exists(options.CardsDirectory)
        && Directory.Exists(options.PowersDirectory)
        && Directory.Exists(options.RelicsDirectory)
        && !string.IsNullOrWhiteSpace(options.CardsJsonPath)
        && !string.IsNullOrWhiteSpace(options.PowersJsonPath)
        && !string.IsNullOrWhiteSpace(options.RelicsJsonPath)
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
        .FirstOrDefault(attribute =>
            attribute.Name.ToString() == attributeName
            || attribute.Name.ToString() == $"{attributeName}Attribute"
            || attribute.Name.ToString().EndsWith($".{attributeName}")
            || attribute.Name.ToString().EndsWith($".{attributeName}Attribute")
        );

static string ToUpperSnakeCase(string value) =>
    Regex.Replace(value, "(?<=[a-z0-9])([A-Z])", "_$1").ToUpperInvariant();

internal sealed record ToolOptions(
    string CardsDirectory,
    string CardsJsonPath,
    string PowersDirectory,
    string PowersJsonPath,
    string RelicsDirectory,
    string RelicsJsonPath,
    string IdPrefix
);

internal sealed record LocalizedModel(string ClassName, IReadOnlyList<LocalizationValue> Values);

internal sealed record LocalizationValue(string Name, string Value);
