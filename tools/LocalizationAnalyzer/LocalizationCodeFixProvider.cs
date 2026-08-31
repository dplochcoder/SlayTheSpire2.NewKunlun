using System.Collections.Immutable;
using System.Composition;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Newtonsoft.Json;

namespace CardLocalizationAnalyzer;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(LocalizationCodeFixProvider)), Shared]
public sealed class LocalizationCodeFixProvider : CodeFixProvider
{
    private const string MissingLocalizationDiagnosticId = "NKLOC004";

    public override ImmutableArray<string> FixableDiagnosticIds =>
        [MissingLocalizationDiagnosticId];

    public override FixAllProvider? GetFixAllProvider() => null;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context
            .Document.GetSyntaxRootAsync(context.CancellationToken)
            .ConfigureAwait(false);
        if (root is null)
            return;

        var diagnostic = context.Diagnostics.First();
        var clazz = root.FindToken(diagnostic.Location.SourceSpan.Start)
            .Parent?.AncestorsAndSelf()
            .OfType<ClassDeclarationSyntax>()
            .FirstOrDefault();
        if (clazz is null)
            return;

        var semanticModel = await context
            .Document.GetSemanticModelAsync(context.CancellationToken)
            .ConfigureAwait(false);
        if (
            semanticModel?.GetDeclaredSymbol(clazz, context.CancellationToken)
                is not INamedTypeSymbol symbol
            || !Localization.TryGetModelKind(symbol, out var kind)
        )
            return;

        var attribute = Localization.FindLocalizationAttribute(clazz, kind);
        if (
            attribute is null
            || !Localization.GetLocalizationStrings(attribute, kind, out var strings)
        )
            return;

        var jsonName = kind switch
        {
            LocalizedModelKind.Card => "cards.json",
            LocalizedModelKind.Power => "powers.json",
            LocalizedModelKind.Relic => "relics.json",
            _ => throw new ArgumentOutOfRangeException(),
        };
        var jsonDocument =
            context.Document.Project.AdditionalDocuments.FirstOrDefault(document =>
                string.Equals(
                    Path.GetFileName(document.FilePath),
                    jsonName,
                    StringComparison.OrdinalIgnoreCase
                )
                && document.FilePath?.IndexOf(
                    $"{Path.DirectorySeparatorChar}eng{Path.DirectorySeparatorChar}",
                    StringComparison.OrdinalIgnoreCase
                ) >= 0
            )
            ?? context.Document.Project.AdditionalDocuments.FirstOrDefault(document =>
                string.Equals(document.Name, jsonName, StringComparison.OrdinalIgnoreCase)
            );
        if (jsonDocument is null)
            return;

        var jsonText = await jsonDocument
            .GetTextAsync(context.CancellationToken)
            .ConfigureAwait(false);
        SortedDictionary<string, string> entries;
        try
        {
            entries =
                JsonConvert.DeserializeObject<SortedDictionary<string, string>>(jsonText.ToString())
                ?? [];
        }
        catch (JsonException)
        {
            return;
        }
        var prefix =
            FindLocalizationPrefix(entries.Keys, symbol.Name)
            ?? ToUpperSnakeCase(context.Document.Project.AssemblyName ?? "NEWKUNLUN");

        context.RegisterCodeFix(
            CodeAction.Create(
                $"Update {jsonName} from localization attribute",
                cancellationToken =>
                    UpdateJsonAsync(
                        context.Document.Project.Solution,
                        jsonDocument.Id,
                        jsonText,
                        prefix,
                        symbol.Name,
                        strings,
                        cancellationToken
                    ),
                equivalenceKey: $"UpdateLocalizationJson.{kind}",
                priority: CodeActionPriority.High
            ),
            diagnostic
        );
    }

    private static Task<Solution> UpdateJsonAsync(
        Solution solution,
        DocumentId documentId,
        SourceText sourceText,
        string prefix,
        string className,
        IReadOnlyList<LocalizationString> strings,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();
        var originalJson = sourceText.ToString();
        var entries =
            JsonConvert.DeserializeObject<SortedDictionary<string, string>>(originalJson) ?? [];
        var modelId = $"{prefix}-{ToUpperSnakeCase(className)}";

        foreach (var localization in strings)
            entries[$"{modelId}.{localization.Name}"] = localization.Value;

        var json = JsonConvert.SerializeObject(entries, Formatting.Indented).Replace("\r\n", "\n");
        var updatedText = SourceText.From(json, sourceText.Encoding);
        return Task.FromResult(solution.WithAdditionalDocumentText(documentId, updatedText));
    }

    private static string? FindLocalizationPrefix(IEnumerable<string> keys, string className)
    {
        var suffix = $"-{ToUpperSnakeCase(className)}.";
        var enumerable = keys.ToList();
        var matchingKey = enumerable.FirstOrDefault(key => key.Contains(suffix));
        if (matchingKey is not null)
            return matchingKey.Substring(0, matchingKey.IndexOf(suffix, StringComparison.Ordinal));

        return enumerable
            .Select(key => Regex.Match(key, "^(?<prefix>[A-Z][A-Z0-9_]*)-[A-Z0-9_]+\\."))
            .Where(match => match.Success)
            .Select(match => match.Groups["prefix"].Value)
            .FirstOrDefault();
    }

    private static string ToUpperSnakeCase(string value) =>
        Regex.Replace(value, "(?<=[a-z0-9])([A-Z])", "_$1").ToUpperInvariant();
}
