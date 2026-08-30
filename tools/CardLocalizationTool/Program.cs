using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

if (!TryReadArguments(args, out var cardsDirectory, out var jsonPath, out var idPrefix))
{
    Console.Error.WriteLine(
        "Usage: CardLocalizationTool --cards <directory> --json <cards.json> --prefix <localization prefix>"
    );
    return 2;
}

SortedDictionary<string, string> generatedJson = new();
foreach (
    var sourcePath in Directory.EnumerateFiles(cardsDirectory, "*.cs", SearchOption.AllDirectories)
)
{
    var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(sourcePath), path: sourcePath);
    foreach (var clazz in tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>())
    {
        var attr = FindLocalizationAttribute(clazz);
        if (attr is null)
            continue;

        var (title, description) = ReadTitleAndDescription(attr);
        var cardId = $"{idPrefix}-{ToUpperSnakeCase(clazz.Identifier.ValueText)}";
        generatedJson[$"{cardId}.description"] = description;
        generatedJson[$"{cardId}.title"] = title;
    }
}

SortedDictionary<string, string> existing = [];
if (File.Exists(jsonPath))
    existing =
        JsonSerializer.Deserialize<SortedDictionary<string, string>>(File.ReadAllText(jsonPath))
        ?? [];

foreach (var (key, value) in generatedJson)
    existing[key] = value;

var json =
    JsonSerializer.Serialize(existing, new JsonSerializerOptions { WriteIndented = true })
    + Environment.NewLine;

if (!File.Exists(jsonPath) || File.ReadAllText(jsonPath) != json)
{
    Directory.CreateDirectory(Path.GetDirectoryName(jsonPath)!);
    File.WriteAllText(jsonPath, json);
    Console.WriteLine($"Updated {jsonPath} from {generatedJson.Count / 2} card annotation(s).");
}
else
{
    Console.WriteLine($"cards.json is current for {generatedJson.Count / 2} card annotation(s).");
}

return 0;

static bool TryReadArguments(
    string[] arguments,
    out string cardsDirectory,
    out string jsonPath,
    out string idPrefix
)
{
    var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    for (var index = 0; index + 1 < arguments.Length; index += 2)
        values[arguments[index]] = arguments[index + 1];

    cardsDirectory = values.GetValueOrDefault("--cards") ?? "";
    jsonPath = values.GetValueOrDefault("--json") ?? "";
    idPrefix = values.GetValueOrDefault("--prefix") ?? "";
    return Directory.Exists(cardsDirectory)
        && !string.IsNullOrWhiteSpace(jsonPath)
        && !string.IsNullOrWhiteSpace(idPrefix);
}

static string ToUpperSnakeCase(string value) =>
    Regex.Replace(value, "(?<=[a-z0-9])([A-Z])", "_$1").ToUpperInvariant();

static AttributeSyntax? FindLocalizationAttribute(ClassDeclarationSyntax clazz) =>
    clazz
        .AttributeLists.SelectMany(list => list.Attributes)
        .FirstOrDefault(attribute =>
            attribute.Name.ToString() is "CardLocalization" or "CardLocalizationAttribute"
            || attribute.Name.ToString().EndsWith(".CardLocalization")
            || attribute.Name.ToString().EndsWith(".CardLocalizationAttribute")
        );

static (string Title, string Description) ReadTitleAndDescription(AttributeSyntax attr)
{
    var args = attr.ArgumentList!.Arguments;
    return (
        ((LiteralExpressionSyntax)args[0].Expression).Token.ValueText,
        ((LiteralExpressionSyntax)args[1].Expression).Token.ValueText
    );
}
