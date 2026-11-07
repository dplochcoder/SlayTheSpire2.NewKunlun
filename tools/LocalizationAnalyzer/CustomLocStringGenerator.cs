using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace CardLocalizationAnalyzer;

[Generator]
public sealed class CustomLocStringGenerator : IIncrementalGenerator
{
    private const string AttributeName =
        "NewKunlun.NewKunlunCode.Localization.CustomLocStringAttribute";

    private static readonly DiagnosticDescriptor InvalidTarget = new(
        "NKLOC008",
        "Invalid CustomLocString target",
        "[CustomLocString] property '{0}' must be a static partial LocString getter in a partial class",
        "Localization",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    private static readonly DiagnosticDescriptor InvalidDescription = new(
        "NKLOC009",
        "Invalid CustomLocString description",
        "[CustomLocString] descriptions must be string literals",
        "Localization",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var properties = context.SyntaxProvider.ForAttributeWithMetadataName(
            AttributeName,
            static (node, _) => node is PropertyDeclarationSyntax,
            static (syntaxContext, _) =>
                new PropertyCandidate(
                    (PropertyDeclarationSyntax)syntaxContext.TargetNode,
                    (IPropertySymbol)syntaxContext.TargetSymbol,
                    syntaxContext.Attributes[0]
                )
        );

        context.RegisterSourceOutput(properties, GenerateProperty);
    }

    private static void GenerateProperty(
        SourceProductionContext context,
        PropertyCandidate candidate
    )
    {
        if (!IsValid(candidate))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    InvalidTarget,
                    candidate.Syntax.Identifier.GetLocation(),
                    candidate.Symbol.Name
                )
            );
            return;
        }

        if (!HasLiteralDescription(candidate.Attribute))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    InvalidDescription,
                    candidate.Attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation()
                        ?? candidate.Syntax.Identifier.GetLocation()
                )
            );
            return;
        }

        if (
            candidate.Attribute.ConstructorArguments.Length != 2
            || candidate.Attribute.ConstructorArguments[0].Value is not int tableValue
            || candidate.Attribute.AttributeClass?.Constructors.FirstOrDefault()
                is not { Parameters.Length: 2 } constructor
        )
            return;

        var tableType = constructor.Parameters[0].Type;
        var tableMember = tableType
            .GetMembers()
            .OfType<IFieldSymbol>()
            .FirstOrDefault(field =>
                field.HasConstantValue && Equals(Convert.ToInt32(field.ConstantValue), tableValue)
            );
        if (tableMember is null)
            return;

        var tableName = ToLowerSnakeCase(tableMember.Name);
        var key =
            $"NEW_KUNLUN-{ToUpperSnakeCase(candidate.Symbol.ContainingType.Name)}-{ToUpperSnakeCase(candidate.Symbol.Name)}";
        var source = Render(candidate.Symbol, tableName, key);
        context.AddSource(
            $"{SanitizeHintName(candidate.Symbol.ToDisplayString())}.CustomLocString.g.cs",
            SourceText.From(source, Encoding.UTF8)
        );
    }

    private static bool HasLiteralDescription(AttributeData attribute)
    {
        if (attribute.ApplicationSyntaxReference?.GetSyntax() is not AttributeSyntax syntax)
            return false;

        var arguments = syntax.ArgumentList?.Arguments;
        if (arguments is null)
            return false;
        var description = arguments.Value.FirstOrDefault(argument =>
            argument.NameColon?.Name.Identifier.ValueText == "description"
        );
        if (description is null && arguments.Value.Count > 1)
            description = arguments.Value[1];
        return description?.Expression is LiteralExpressionSyntax literal
            && literal.IsKind(SyntaxKind.StringLiteralExpression);
    }

    private static bool IsValid(PropertyCandidate candidate)
    {
        var syntax = candidate.Syntax;
        var symbol = candidate.Symbol;
        return symbol.IsStatic
            && syntax.Modifiers.Any(SyntaxKind.PartialKeyword)
            && symbol.Type.Name == "LocString"
            && symbol.ContainingType.TypeKind == TypeKind.Class
            && syntax.ExpressionBody is null
            && syntax.AccessorList is { Accessors.Count: 1 } accessors
            && accessors.Accessors[0].IsKind(SyntaxKind.GetAccessorDeclaration)
            && accessors.Accessors[0].Body is null
            && accessors.Accessors[0].ExpressionBody is null
            && AllContainingTypesArePartial(syntax);
    }

    private static bool AllContainingTypesArePartial(PropertyDeclarationSyntax property)
    {
        for (
            var type = property.Parent as TypeDeclarationSyntax;
            type is not null;
            type = type.Parent as TypeDeclarationSyntax
        )
        {
            if (!type.Modifiers.Any(SyntaxKind.PartialKeyword))
                return false;
        }
        return property.Parent is ClassDeclarationSyntax;
    }

    private static string Render(IPropertySymbol property, string tableName, string key)
    {
        var source = new StringBuilder("// <auto-generated />\n#nullable enable\n");
        if (!property.ContainingNamespace.IsGlobalNamespace)
            source
                .Append("namespace ")
                .Append(property.ContainingNamespace.ToDisplayString())
                .Append(";\n\n");

        var containingTypes = new Stack<INamedTypeSymbol>();
        for (var type = property.ContainingType; type is not null; type = type.ContainingType)
            containingTypes.Push(type);

        var indentation = string.Empty;
        foreach (var type in containingTypes)
        {
            source
                .Append(indentation)
                .Append(type.IsStatic ? "static partial class " : "partial class ")
                .Append(type.Name);
            if (type.TypeParameters.Length > 0)
                source
                    .Append('<')
                    .Append(
                        string.Join(", ", type.TypeParameters.Select(parameter => parameter.Name))
                    )
                    .Append('>');
            source.Append("\n").Append(indentation).Append("{\n");
            indentation += "    ";
        }

        source
            .Append(indentation)
            .Append(AccessibilityText(property.DeclaredAccessibility))
            .Append(" static partial global::MegaCrit.Sts2.Core.Localization.LocString ")
            .Append(property.Name)
            .Append(" => new(")
            .Append(SymbolDisplay.FormatLiteral(tableName, quote: true))
            .Append(", ")
            .Append(SymbolDisplay.FormatLiteral(key, quote: true))
            .Append(");\n");

        while (containingTypes.Count > 0)
        {
            containingTypes.Pop();
            indentation = indentation.Substring(0, indentation.Length - 4);
            source.Append(indentation).Append("}\n");
        }
        return source.ToString();
    }

    private static string AccessibilityText(Accessibility accessibility) =>
        accessibility switch
        {
            Accessibility.Public => "public",
            Accessibility.Internal => "internal",
            Accessibility.Private => "private",
            Accessibility.Protected => "protected",
            Accessibility.ProtectedAndInternal => "private protected",
            Accessibility.ProtectedOrInternal => "protected internal",
            _ => "private",
        };

    private static string ToUpperSnakeCase(string value) =>
        Regex.Replace(value, "(?<=[a-z0-9])([A-Z])", "_$1").ToUpperInvariant();

    private static string ToLowerSnakeCase(string value) =>
        Regex.Replace(value, "(?<=[a-z0-9])([A-Z])", "_$1").ToLowerInvariant();

    private static string SanitizeHintName(string name)
    {
        var result = new StringBuilder(name.Length);
        foreach (var character in name)
            result.Append(
                char.IsLetterOrDigit(character) || character is '.' or '_' ? character : '_'
            );
        return result.ToString();
    }

    private sealed class PropertyCandidate(
        PropertyDeclarationSyntax syntax,
        IPropertySymbol symbol,
        AttributeData attribute
    )
    {
        public PropertyDeclarationSyntax Syntax { get; } = syntax;
        public IPropertySymbol Symbol { get; } = symbol;
        public AttributeData Attribute { get; } = attribute;
    }
}
