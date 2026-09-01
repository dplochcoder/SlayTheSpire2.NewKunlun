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

    public static IReadOnlyList<CardEntry> ReadCards(string repositoryRoot)
    {
        var cardsDirectory = Path.Combine(repositoryRoot, "NewKunlun", "NewKunlunCode", "Cards");
        var portraitsDirectory = Path.Combine(
            repositoryRoot,
            "NewKunlun",
            "NewKunlun",
            "images",
            "card_portraits"
        );
        var result = new List<CardEntry>();

        foreach (
            var path in Directory.EnumerateFiles(
                cardsDirectory,
                "*.cs",
                SearchOption.AllDirectories
            )
        )
        {
            var root = CSharpSyntaxTree.ParseText(File.ReadAllText(path), path: path).GetRoot();
            foreach (var clazz in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
            {
                if (!DirectlyExtends(clazz, "NewKunlunCard"))
                    continue;
                var attribute = clazz
                    .AttributeLists.SelectMany(list => list.Attributes)
                    .FirstOrDefault(candidate =>
                        candidate
                            .Name.ToString()
                            .EndsWith("CardLocalization", StringComparison.Ordinal)
                        || candidate
                            .Name.ToString()
                            .EndsWith("CardLocalizationAttribute", StringComparison.Ordinal)
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
                    Path.Combine(portraitsDirectory, fileName),
                    Path.Combine(portraitsDirectory, "big", fileName)
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
