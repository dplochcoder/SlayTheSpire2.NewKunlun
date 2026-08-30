using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

if (!TryReadArguments(args, out var options))
{
    Console.Error.WriteLine(
        "Usage: CardLocalizationTool --cards <directory> --cards-json <cards.json> "
            + "--powers <directory> --powers-json <powers.json> --prefix <localization prefix>"
    );
    return 2;
}

var cards = ReadModels(options.CardsDirectory, "NewKunlunCard", "CardLocalization", 2);
var powers = ReadModels(options.PowersDirectory, "NewKunlunPower", "PowerLocalization", 3);

UpdateJson(options.CardsJsonPath, options.IdPrefix, cards, "card");
UpdateJson(options.PowersJsonPath, options.IdPrefix, powers, "power");
return 0;

static IReadOnlyList<LocalizedModel> ReadModels(
    string sourceDirectory,
    string baseTypeName,
    string attributeName,
    int valueCount
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
            if (attr?.ArgumentList?.Arguments.Count != valueCount)
                continue;

            var values = attr
                .ArgumentList.Arguments.Select(argument =>
                    ((LiteralExpressionSyntax)argument.Expression).Token.ValueText
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
        generatedJson[$"{modelId}.description"] = model.Values[1];
        if (model.Values.Count == 3)
            generatedJson[$"{modelId}.smartDescription"] = model.Values[2];
        generatedJson[$"{modelId}.title"] = model.Values[0];
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
        values.GetValueOrDefault("--prefix") ?? ""
    );
    return Directory.Exists(options.CardsDirectory)
        && Directory.Exists(options.PowersDirectory)
        && !string.IsNullOrWhiteSpace(options.CardsJsonPath)
        && !string.IsNullOrWhiteSpace(options.PowersJsonPath)
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
    string IdPrefix
);

internal sealed record LocalizedModel(string ClassName, IReadOnlyList<string> Values);
