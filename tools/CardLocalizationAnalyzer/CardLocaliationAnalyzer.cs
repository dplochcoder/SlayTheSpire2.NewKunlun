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
        "Invalid model localization",
        "Localization fields must be string literals",
        "Localization",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Localization is generated at build time and requires constant string literals."
    );

    private static readonly DiagnosticDescriptor UnknownVariable = new(
        UnknownVariableId,
        "Unknown model description variable",
        "Description for '{0}' references unknown dynamic variable '{1}'; valid names: {2}{3}",
        "Localization",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Variables referenced by a model description must be declared in CanonicalVars."
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
        if (context.SemanticModel.GetDeclaredSymbol(clazz) is not { } classSymbol)
            return;
        if (!CardLocalization.TryGetModelKind(classSymbol, out var kind))
            return;

        var attr = CardLocalization.FindLocalizationAttribute(clazz, kind);
        if (attr is null)
            return;

        if (!CardLocalization.GetLocalizationStrings(attr, kind, out var localizationStrings))
        {
            context.ReportDiagnostic(Diagnostic.Create(InvalidLocalization, attr.GetLocation()));
            return;
        }

        var validVariables = DynamicVariables.FindDynamicVariables(clazz);
        for (var index = 1; index < localizationStrings.Count; index++)
        {
            HashSet<string> allowedVariables = [.. validVariables];
            switch (kind)
            {
                case LocalizedModelKind.Card:
                    allowedVariables.Add("IfUpgraded");
                    break;
                case LocalizedModelKind.Power:
                    allowedVariables.Add("Amount");
                    break;
            }

            var descriptionLocation = attr.ArgumentList!.Arguments[index].Expression.GetLocation();
            foreach (
                var variableName in DynamicVariables.ParseReferencedVariables(
                    localizationStrings[index]
                )
            )
            {
                if (allowedVariables.Contains(variableName))
                    continue;

                var suggestion = EditDistance.FindClosest(
                    variableName,
                    allowedVariables,
                    out var bestMatch
                )
                    ? $"; suggested correction: '{bestMatch}'"
                    : string.Empty;
                var validNames =
                    allowedVariables.Count == 0
                        ? "(none)"
                        : string.Join(", ", allowedVariables.OrderBy(name => name));

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
}
