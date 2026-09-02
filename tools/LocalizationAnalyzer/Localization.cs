using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CardLocalizationAnalyzer;

public enum LocalizedModelKind
{
    Card,
    Power,
    Relic,
}

public static class Localization
{
    private static readonly IReadOnlyDictionary<LocalizedModelKind, string> AttributeNames =
        new Dictionary<LocalizedModelKind, string>
        {
            [LocalizedModelKind.Card] = "CardLocalization",
            [LocalizedModelKind.Power] = "PowerLocalization",
            [LocalizedModelKind.Relic] = "RelicLocalization",
        };

    public static bool TryGetModelKind(INamedTypeSymbol symbol, out LocalizedModelKind kind)
    {
        for (var current = symbol.BaseType; current is not null; current = current.BaseType)
        {
            if (current.Name == "NewKunlunCard")
            {
                kind = LocalizedModelKind.Card;
                return true;
            }

            if (current.Name == "NewKunlunPower")
            {
                kind = LocalizedModelKind.Power;
                return true;
            }

            if (current.Name == "NewKunlunRelic")
            {
                kind = LocalizedModelKind.Relic;
                return true;
            }
        }

        kind = default;
        return false;
    }

    public static AttributeSyntax? FindLocalizationAttribute(
        ClassDeclarationSyntax clazz,
        LocalizedModelKind kind
    ) =>
        clazz
            .AttributeLists.SelectMany(list => list.Attributes)
            .FirstOrDefault(attribute => IsAttribute(attribute, GetAttributeName(kind)));

    public static IEnumerable<AttributeSyntax> FindLocalizationAttributes(
        ClassDeclarationSyntax clazz
    ) =>
        clazz
            .AttributeLists.SelectMany(list => list.Attributes)
            .Where(attribute => AttributeNames.Values.Any(name => IsAttribute(attribute, name)));

    public static string GetAttributeName(LocalizedModelKind kind) =>
        AttributeNames.TryGetValue(kind, out var name)
            ? name
            : throw new ArgumentOutOfRangeException(nameof(kind));

    public static string GetModelName(LocalizedModelKind kind) =>
        kind switch
        {
            LocalizedModelKind.Card => "NewKunlunCard",
            LocalizedModelKind.Power => "NewKunlunPower",
            LocalizedModelKind.Relic => "NewKunlunRelic",
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };

    public static bool GetLocalizationStrings(
        AttributeSyntax attr,
        LocalizedModelKind kind,
        out IReadOnlyList<LocalizationString> values
    )
    {
        IReadOnlyList<string> requiredParameterNames = kind switch
        {
            LocalizedModelKind.Card => ["title", "description"],
            LocalizedModelKind.Power => ["title", "description"],
            LocalizedModelKind.Relic => ["title", "description", "flavor"],
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };
        if (attr.ArgumentList is not { } argumentList)
        {
            values = Array.Empty<LocalizationString>();
            return false;
        }

        var result = new List<LocalizationString>(argumentList.Arguments.Count);
        for (var index = 0; index < argumentList.Arguments.Count; index++)
        {
            var argument = argumentList.Arguments[index];
            var parameterName = argument.NameColon?.Name.Identifier.ValueText;
            if (!TryReadString(argument.Expression, out var value))
            {
                values = Array.Empty<LocalizationString>();
                return false;
            }
            if (
                parameterName is null
                || result.Any(localization => localization.Name == parameterName)
            )
            {
                values = Array.Empty<LocalizationString>();
                return false;
            }

            result.Add(new LocalizationString(parameterName, value, argument.Expression));
        }

        // smartDescription is required but often can be copied.
        if (
            kind == LocalizedModelKind.Power
            && result.All(l => l.Name != "smartDescription")
            && result.FirstOrDefault(l => l.Name == "description") is { } description
        )
            result.Add(
                new LocalizationString(
                    "smartDescription",
                    description.Value,
                    description.Expression
                )
            );

        if (
            requiredParameterNames.Any(required =>
                result.All(localization => localization.Name != required)
            )
        )
        {
            values = Array.Empty<LocalizationString>();
            return false;
        }

        values = result;
        return true;
    }

    private static bool IsAttribute(AttributeSyntax attribute, string name) =>
        attribute.Name.ToString() == name
        || attribute.Name.ToString() == $"{name}Attribute"
        || attribute.Name.ToString().EndsWith($".{name}")
        || attribute.Name.ToString().EndsWith($".{name}Attribute");

    private static bool TryReadString(ExpressionSyntax expression, out string value)
    {
        if (
            expression is LiteralExpressionSyntax literal
            && literal.IsKind(SyntaxKind.StringLiteralExpression)
        )
        {
            value = literal.Token.ValueText;
            return true;
        }

        value = "";
        return false;
    }
}

public sealed class LocalizationString(string name, string value, ExpressionSyntax expression)
{
    public string Name { get; } = name;
    public string Value { get; } = value;
    public ExpressionSyntax Expression { get; } = expression;
}
