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

public static class CardLocalization
{
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
            .FirstOrDefault(attribute =>
                IsAttribute(
                    attribute,
                    kind switch
                    {
                        LocalizedModelKind.Card => "CardLocalization",
                        LocalizedModelKind.Power => "PowerLocalization",
                        LocalizedModelKind.Relic => "RelicLocalization",
                        _ => throw new ArgumentOutOfRangeException(nameof(kind)),
                    }
                )
            );

    public static bool GetLocalizationStrings(
        AttributeSyntax attr,
        LocalizedModelKind kind,
        out IReadOnlyList<string> values
    )
    {
        var minimumCount = kind == LocalizedModelKind.Card ? 2 : 3;
        var maximumCount = kind == LocalizedModelKind.Power ? 4 : minimumCount;
        if (attr.ArgumentList is not { } argumentList)
        {
            values = Array.Empty<string>();
            return false;
        }

        var argumentCount = argumentList.Arguments.Count;
        if (argumentCount < minimumCount || argumentCount > maximumCount)
        {
            values = Array.Empty<string>();
            return false;
        }

        var result = new List<string>(argumentCount);
        foreach (var argument in argumentList.Arguments)
        {
            if (!TryReadString(argument.Expression, out var value))
            {
                values = Array.Empty<string>();
                return false;
            }
            result.Add(value);
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
