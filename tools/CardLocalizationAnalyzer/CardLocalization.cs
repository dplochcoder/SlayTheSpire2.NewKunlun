using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CardLocalizationAnalyzer;

public static class CardLocalization
{
    public static AttributeSyntax? FindLocalizationAttribute(ClassDeclarationSyntax clazz) =>
        clazz
            .AttributeLists.SelectMany(list => list.Attributes)
            .FirstOrDefault(attribute =>
                attribute.Name.ToString() is "CardLocalization" or "CardLocalizationAttribute"
                || attribute.Name.ToString().EndsWith(".CardLocalization")
                || attribute.Name.ToString().EndsWith(".CardLocalizationAttribute")
            );

    public static bool GetTitleAndDescription(
        AttributeSyntax attr,
        out string title,
        out string description
    )
    {
        title = "";
        description = "";

        return attr.ArgumentList?.Arguments
                is SeparatedSyntaxList<AttributeArgumentSyntax> { Count: 2 } args
            && TryReadString(args[0].Expression, out title)
            && TryReadString(args[1].Expression, out description);
    }

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
