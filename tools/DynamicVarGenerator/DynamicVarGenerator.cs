using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace CardDynamicVarGenerator;

[Generator]
public sealed class DynamicVarGenerator : IIncrementalGenerator
{
    private static readonly DiagnosticDescriptor ClassMustBePartial = new(
        "NKVAR001",
        "Localized model class must be partial",
        "Localized model class '{0}' declares CanonicalVars and must be partial so variable properties can be generated",
        "CodeGeneration",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    private static readonly DiagnosticDescriptor VariableNeedsName = new(
        "NKVAR002",
        "Dynamic variable needs a property name",
        "'{0}' must receive its generated property name through nameof(PropertyName)",
        "CodeGeneration",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var candidates = context
            .SyntaxProvider.CreateSyntaxProvider(
                static (node, _) =>
                    node is PropertyDeclarationSyntax { Identifier.ValueText: "CanonicalVars" }
                    && node.Parent is ClassDeclarationSyntax,
                static (syntaxContext, _) =>
                    (ClassDeclarationSyntax)((PropertyDeclarationSyntax)syntaxContext.Node).Parent!
            )
            .Collect();

        context.RegisterSourceOutput(
            context.CompilationProvider.Combine(candidates),
            static (productionContext, input) =>
                GenerateAll(productionContext, input.Left, input.Right)
        );
    }

    private static void GenerateAll(
        SourceProductionContext context,
        Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax> candidates
    )
    {
        foreach (
            var clazz in candidates
                .GroupBy(candidate => (candidate.SyntaxTree, candidate.SpanStart))
                .Select(group => group.First())
        )
            GenerateForClass(context, compilation, clazz);
    }

    private static void GenerateForClass(
        SourceProductionContext context,
        Compilation compilation,
        ClassDeclarationSyntax clazz
    )
    {
        var canonicalVars = clazz
            .Members.OfType<PropertyDeclarationSyntax>()
            .First(property => property.Identifier.ValueText == "CanonicalVars");

        var semanticModel = compilation.GetSemanticModel(clazz.SyntaxTree);
        if (semanticModel.GetDeclaredSymbol(clazz) is not { } classSymbol)
            return;
        if (!IsLocalizedModel(classSymbol))
            return;

        if (!clazz.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    ClassMustBePartial,
                    clazz.Identifier.GetLocation(),
                    clazz.Identifier.ValueText
                )
            );
            return;
        }

        Dictionary<string, VariableProperty> variables = [];
        foreach (
            var creation in canonicalVars.DescendantNodes().OfType<ObjectCreationExpressionSyntax>()
        )
        {
            if (semanticModel.GetTypeInfo(creation).Type is not INamedTypeSymbol variableType)
                continue;
            if (!variableType.Name.EndsWith("Var", StringComparison.Ordinal))
                continue;

            var propertyName = FindExplicitName(creation);
            if (propertyName is null && variableType.Name != "DynamicVar")
                propertyName = variableType.Name.Substring(0, variableType.Name.Length - 3);

            if (propertyName is null)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        VariableNeedsName,
                        creation.GetLocation(),
                        variableType.ToDisplayString()
                    )
                );
                continue;
            }

            variables[propertyName] = new VariableProperty(propertyName, variableType);
        }

        if (variables.Count == 0)
            return;

        var source = RenderClass(classSymbol, variables.Values.OrderBy(variable => variable.Name));
        context.AddSource(
            $"{SanitizeHintName(classSymbol.ToDisplayString())}.CanonicalVars.g.cs",
            SourceText.From(source, Encoding.UTF8)
        );
    }

    private static string? FindExplicitName(ObjectCreationExpressionSyntax creation)
    {
        var firstArgument = creation.ArgumentList?.Arguments.FirstOrDefault()?.Expression;
        return firstArgument switch
        {
            InvocationExpressionSyntax
            {
                Expression: IdentifierNameSyntax { Identifier.ValueText: "nameof" },
                ArgumentList.Arguments.Count: 1
            } invocation => invocation.ArgumentList.Arguments[0].Expression switch
            {
                IdentifierNameSyntax identifier => identifier.Identifier.ValueText,
                MemberAccessExpressionSyntax member => member.Name.Identifier.ValueText,
                _ => null,
            },
            LiteralExpressionSyntax literal
                when literal.IsKind(SyntaxKind.StringLiteralExpression) => literal.Token.ValueText,
            _ => null,
        };
    }

    private static bool IsLocalizedModel(INamedTypeSymbol symbol)
    {
        for (var current = symbol.BaseType; current is not null; current = current.BaseType)
        {
            if (current.Name is "NewKunlunCard" or "NewKunlunPower" or "NewKunlunRelic")
                return true;
        }
        return false;
    }

    private static string RenderClass(
        INamedTypeSymbol classSymbol,
        IEnumerable<VariableProperty> variables
    )
    {
        var source = new StringBuilder("// <auto-generated />\n#nullable enable\n");
        if (!classSymbol.ContainingNamespace.IsGlobalNamespace)
            source
                .Append("namespace ")
                .Append(classSymbol.ContainingNamespace.ToDisplayString())
                .Append(";\n\n");

        source.Append("partial class ").Append(classSymbol.Name);
        if (classSymbol.TypeParameters.Length > 0)
            source
                .Append('<')
                .Append(string.Join(", ", classSymbol.TypeParameters.Select(type => type.Name)))
                .Append('>');
        source.Append("\n{\n");

        foreach (var variable in variables)
        {
            var typeName = variable.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            var key = SymbolDisplay.FormatLiteral(variable.Name, quote: true);
            source
                .Append("    public ")
                .Append(typeName)
                .Append(' ')
                .Append(variable.Name)
                .Append(" =>\n        ");
            if (variable.Type.Name != "DynamicVar")
                source.Append('(').Append(typeName).Append(')');
            source.Append("DynamicVars[").Append(key).Append("];\n");
        }

        source.Append("}\n");
        return source.ToString();
    }

    private static string SanitizeHintName(string name)
    {
        var result = new StringBuilder(name.Length);
        foreach (var character in name)
            result.Append(
                char.IsLetterOrDigit(character) || character is '.' or '_' ? character : '_'
            );
        return result.ToString();
    }

    private sealed class VariableProperty(string name, INamedTypeSymbol type)
    {
        public string Name { get; } = name;
        public INamedTypeSymbol Type { get; } = type;
    }
}
