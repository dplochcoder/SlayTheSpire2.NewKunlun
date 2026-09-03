using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CardLocalizationAnalyzer;

public static class DynamicVariables
{
    private static readonly Regex PlaceholderPattern = new(
        @"(?<!\{)\{(?<name>[A-Za-z_][A-Za-z0-9_]*)(?::[^{}]+)?\}(?!\})",
        RegexOptions.Compiled
    );

    public static HashSet<string> FindDynamicVariables(ClassDeclarationSyntax clazz)
    {
        HashSet<string> names = [];
        var canonicalVars = clazz
            .Members.OfType<PropertyDeclarationSyntax>()
            .FirstOrDefault(property => property.Identifier.ValueText == "CanonicalVars");
        if (canonicalVars is null)
            return names;

        foreach (
            var creation in canonicalVars.DescendantNodes().OfType<ObjectCreationExpressionSyntax>()
        )
        {
            var typeName = GetUnqualifiedTypeName(creation.Type);
            if (typeName is null)
                continue;

            if (!typeName.EndsWith("Var"))
                continue;

            var defaultName = typeName.Substring(0, typeName.Length - 3);
            var firstArgument = creation.ArgumentList?.Arguments.FirstOrDefault()?.Expression;
            switch (firstArgument)
            {
                case InvocationExpressionSyntax invocation
                    when invocation.Expression
                        is IdentifierNameSyntax { Identifier.ValueText: "nameof" }
                        && invocation.ArgumentList.Arguments.Count == 1:
                    names.Add(
                        invocation.ArgumentList.Arguments[0].Expression.ToString().Split('.').Last()
                    );
                    break;
                case LiteralExpressionSyntax stringLiteral
                    when stringLiteral.IsKind(SyntaxKind.StringLiteralExpression):
                    names.Add(stringLiteral.Token.ValueText);
                    break;
                default:
                    if (typeName != "DynamicVar")
                        names.Add(defaultName);
                    break;
            }
        }

        return names;
    }

    private static string? GetUnqualifiedTypeName(TypeSyntax type) =>
        type switch
        {
            SimpleNameSyntax simpleName => simpleName.Identifier.ValueText,
            QualifiedNameSyntax qualifiedName => GetUnqualifiedTypeName(qualifiedName.Right),
            AliasQualifiedNameSyntax aliasQualifiedName => GetUnqualifiedTypeName(
                aliasQualifiedName.Name
            ),
            _ => null,
        };

    public static IEnumerable<string> ParseReferencedVariables(string description) =>
        PlaceholderPattern
            .Matches(description)
            .Cast<Match>()
            .Select(match => match.Groups["name"].Value)
            .Distinct();
}
