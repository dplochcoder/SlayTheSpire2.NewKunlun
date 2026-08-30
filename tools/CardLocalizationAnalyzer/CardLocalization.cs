using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CardLocalizationAnalyzer;

public enum LocalizedModelKind
{
    Card,
    Power,
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
                    kind == LocalizedModelKind.Card ? "CardLocalization" : "PowerLocalization"
                )
            );

    public static bool GetLocalizationStrings(
        AttributeSyntax attr,
        LocalizedModelKind kind,
        out IReadOnlyList<string> values
    )
    {
        var expectedCount = kind == LocalizedModelKind.Card ? 2 : 3;
        if (attr.ArgumentList?.Arguments.Count != expectedCount)
        {
            values = Array.Empty<string>();
            return false;
        }

        var result = new List<string>(expectedCount);
        foreach (var argument in attr.ArgumentList.Arguments)
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
