using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CardArtClipper;

internal static class CardDiscovery
{
    public static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (
                Directory.Exists(
                    Path.Combine(directory.FullName, "NewKunlun", "NewKunlunCode", "Cards")
                )
            )
                return directory.FullName;
            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not locate the repository root containing NewKunlun/NewKunlunCode/Cards."
        );
    }

    public static IReadOnlyList<CardEntry> ReadEntries(string repositoryRoot, AssetKind kind)
    {
        var (
            codeFolder,
            baseClass,
            localizationAttribute,
            imageFolder,
            smallWidth,
            smallHeight,
            largeWidth,
            largeHeight
        ) = kind switch
        {
            AssetKind.Card => (
                "Cards",
                "NewKunlunCard",
                "CardLocalization",
                "card_portraits",
                250,
                190,
                1000,
                760
            ),
            AssetKind.Power => (
                "Powers",
                "NewKunlunPower",
                "PowerLocalization",
                "powers",
                64,
                64,
                256,
                256
            ),
            AssetKind.Relic => (
                "Relics",
                "NewKunlunRelic",
                "RelicLocalization",
                "relics",
                94,
                94,
                256,
                256
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };
        var modelsDirectory = Path.Combine(
            repositoryRoot,
            "NewKunlun",
            "NewKunlunCode",
            codeFolder
        );
        var imagesDirectory = Path.Combine(
            repositoryRoot,
            "NewKunlun",
            "NewKunlun",
            "images",
            imageFolder
        );
        var result = new List<CardEntry>();

        foreach (
            var path in Directory.EnumerateFiles(
                modelsDirectory,
                "*.cs",
                SearchOption.AllDirectories
            )
        )
        {
            var root = CSharpSyntaxTree.ParseText(File.ReadAllText(path), path: path).GetRoot();
            foreach (var clazz in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
            {
                if (!DirectlyExtends(clazz, baseClass))
                    continue;
                var attribute = clazz
                    .AttributeLists.SelectMany(list => list.Attributes)
                    .FirstOrDefault(candidate =>
                        candidate
                            .Name.ToString()
                            .EndsWith(localizationAttribute, StringComparison.Ordinal)
                        || candidate
                            .Name.ToString()
                            .EndsWith($"{localizationAttribute}Attribute", StringComparison.Ordinal)
                    );
                var title =
                    attribute
                        ?.ArgumentList?.Arguments.FirstOrDefault(argument =>
                            argument.NameColon?.Name.Identifier.ValueText == "title"
                        )
                        ?.Expression as LiteralExpressionSyntax;
                if (title is null || !title.IsKind(SyntaxKind.StringLiteralExpression))
                    continue;

                var className = clazz.Identifier.ValueText;
                var fileName = $"{ToLowerSnakeCase(className)}.png";
                var entry = new CardEntry(
                    className,
                    title.Token.ValueText,
                    kind,
                    Path.Combine(imagesDirectory, fileName),
                    Path.Combine(imagesDirectory, "big", fileName),
                    smallWidth,
                    smallHeight,
                    largeWidth,
                    largeHeight
                );
                entry.RefreshStatus();
                result.Add(entry);
            }
        }

        return result
            .OrderBy(card => card.Title, StringComparer.CurrentCultureIgnoreCase)
            .ToArray();
    }

    private static bool DirectlyExtends(ClassDeclarationSyntax clazz, string baseName) =>
        clazz.BaseList?.Types.Any(type =>
            type.Type.ToString().Split('.').Last().Split('(').First() == baseName
        ) == true;

    private static string ToLowerSnakeCase(string value) =>
        Regex.Replace(value, "(?<=[a-z0-9])([A-Z])", "_$1").ToLowerInvariant();
}
