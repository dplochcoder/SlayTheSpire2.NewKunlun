using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;

namespace CardLocalizationAnalyzer;

internal static class LocalizationJson
{
    public static bool ContainsCurrentLocalization(
        ImmutableArray<AdditionalText> additionalFiles,
        LocalizedModelKind kind,
        string className,
        IReadOnlyList<LocalizationString> strings,
        CancellationToken cancellationToken
    )
    {
        var fileName = kind switch
        {
            LocalizedModelKind.Card => "cards.json",
            LocalizedModelKind.Power => "powers.json",
            LocalizedModelKind.Relic => "relics.json",
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };
        var file =
            additionalFiles.FirstOrDefault(file =>
                string.Equals(
                    Path.GetFileName(file.Path),
                    fileName,
                    StringComparison.OrdinalIgnoreCase
                )
                && file.Path.IndexOf(
                    $"{Path.DirectorySeparatorChar}eng{Path.DirectorySeparatorChar}",
                    StringComparison.OrdinalIgnoreCase
                ) >= 0
            )
            ?? additionalFiles.FirstOrDefault(file =>
                string.Equals(
                    Path.GetFileName(file.Path),
                    fileName,
                    StringComparison.OrdinalIgnoreCase
                )
            );
        var text = file?.GetText(cancellationToken)?.ToString();
        if (text is null)
            return false;

        SortedDictionary<string, string> entries;
        try
        {
            entries = JsonConvert.DeserializeObject<SortedDictionary<string, string>>(text) ?? [];
        }
        catch (JsonException)
        {
            return false;
        }
        var classSuffix = $"-{ToUpperSnakeCase(className)}.";
        var prefix =
            entries
                .Keys.Select(key =>
                    key.IndexOf(classSuffix, StringComparison.Ordinal) is var index && index > 0
                        ? key.Substring(0, index)
                        : null
                )
                .FirstOrDefault(value => value is not null)
            ?? entries
                .Keys.Select(key => Regex.Match(key, "^(?<prefix>[A-Z][A-Z0-9_]*)-[A-Z0-9_]+\\."))
                .Where(match => match.Success)
                .Select(match => match.Groups["prefix"].Value)
                .FirstOrDefault();
        if (prefix is null)
            return false;

        var modelId = $"{prefix}-{ToUpperSnakeCase(className)}";
        return strings.All(localization =>
            entries.TryGetValue($"{modelId}.{localization.Name}", out var value)
            && value == localization.Value
        );
    }

    private static string ToUpperSnakeCase(string value) =>
        Regex.Replace(value, "(?<=[a-z0-9])([A-Z])", "_$1").ToUpperInvariant();
}
