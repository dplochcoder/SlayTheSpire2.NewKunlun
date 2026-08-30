using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CardLocalizationAnalyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class CardLocaliationAnalyzer : DiagnosticAnalyzer
{
    public const string InvalidLocalizationId = "NKLOC001";
    public const string UnknownVariableId = "NKLOC002";

    private static readonly DiagnosticDescriptor InvalidLocalization = new(
        InvalidLocalizationId,
        "Invalid card localization",
        "CardLocalization title and description must be string literals",
        "Localization",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Card localization is generated at build time and requires constant string literals."
    );

    private static readonly DiagnosticDescriptor UnknownVariable = new(
        UnknownVariableId,
        "Unknown card description variable",
        "Description for '{0}' references unknown dynamic variable '{1}'; valid names: {2}{3}",
        "Localization",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Variables referenced by a card description must be declared in CanonicalVars."
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(InvalidLocalization, UnknownVariable);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeClass, SyntaxKind.ClassDeclaration);
    }

    private static void AnalyzeClass(SyntaxNodeAnalysisContext context)
    {
        var clazz = (ClassDeclarationSyntax)context.Node;
        var attr = CardLocalization.FindLocalizationAttribute(clazz);
        if (attr is null)
            return;

        if (!CardLocalization.GetTitleAndDescription(attr, out _, out var description))
        {
            context.ReportDiagnostic(Diagnostic.Create(InvalidLocalization, attr.GetLocation()));
            return;
        }

        var validVariables = DynamicVariables.FindDynamicVariables(clazz);
        var descriptionLocation = attr.ArgumentList!.Arguments[1].Expression.GetLocation();
        foreach (var variableName in DynamicVariables.ParseReferencedVariables(description))
        {
            if (validVariables.Contains(variableName))
                continue;

            var suggestion = EditDistance.FindClosest(
                variableName,
                validVariables,
                out var bestMatch
            )
                ? $"; suggested correction: '{bestMatch}'"
                : string.Empty;
            var validNames =
                validVariables.Count == 0
                    ? "(none)"
                    : string.Join(", ", validVariables.OrderBy(name => name));

            context.ReportDiagnostic(
                Diagnostic.Create(
                    UnknownVariable,
                    descriptionLocation,
                    clazz.Identifier.ValueText,
                    variableName,
                    validNames,
                    suggestion
                )
            );
        }
    }
}
