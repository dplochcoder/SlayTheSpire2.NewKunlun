using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CardLocalizationAnalyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class LocaliationAnalyzer : DiagnosticAnalyzer
{
    public const string InvalidLocalizationId = "NKLOC001";
    public const string UnknownVariableId = "NKLOC002";
    public const string UnnamedArgumentId = "NKLOC003";
    public const string MissingOrMismatchedLocalizationId = "NKLOC004";

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

    private static readonly DiagnosticDescriptor UnnamedArgument = new(
        UnnamedArgumentId,
        "Localization argument must be named",
        "Localization arguments must use named-argument syntax",
        "Localization",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Named arguments keep localization fields independent of constructor parameter order."
    );

    private static readonly DiagnosticDescriptor MissingOrMismatchedLocalization = new(
        MissingOrMismatchedLocalizationId,
        "Missing or mismatched localization attribute",
        "'{0}' derives from {1} and must use [{2}] localization; {3}",
        "Localization",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Every localized model must declare the localization attribute matching its model hierarchy."
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(
            InvalidLocalization,
            UnknownVariable,
            UnnamedArgument,
            MissingOrMismatchedLocalization
        );

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
        if (!Localization.TryGetModelKind(classSymbol, out var kind))
            return;

        var expectedAttributeName = Localization.GetAttributeName(kind);
        var localizationAttributes = Localization.FindLocalizationAttributes(clazz).ToArray();
        var attr = Localization.FindLocalizationAttribute(clazz, kind);
        foreach (
            var mismatchedAttribute in localizationAttributes.Where(candidate => candidate != attr)
        )
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    MissingOrMismatchedLocalization,
                    mismatchedAttribute.GetLocation(),
                    classSymbol.Name,
                    Localization.GetModelName(kind),
                    expectedAttributeName,
                    $"found [{mismatchedAttribute.Name}]"
                )
            );
        }

        if (attr is null)
        {
            if (localizationAttributes.Length == 0)
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        MissingOrMismatchedLocalization,
                        clazz.Identifier.GetLocation(),
                        classSymbol.Name,
                        Localization.GetModelName(kind),
                        expectedAttributeName,
                        "no localization attribute was found"
                    )
                );
            return;
        }

        var unnamedArguments =
            attr.ArgumentList?.Arguments.Where(argument => argument.NameColon is null).ToArray()
            ?? [];
        foreach (var argument in unnamedArguments)
            context.ReportDiagnostic(
                Diagnostic.Create(UnnamedArgument, argument.Expression.GetLocation())
            );
        if (unnamedArguments.Length > 0)
            return;

        if (!Localization.GetLocalizationStrings(attr, kind, out var localizationStrings))
        {
            context.ReportDiagnostic(Diagnostic.Create(InvalidLocalization, attr.GetLocation()));
            return;
        }

        var validVariables = DynamicVariables.FindDynamicVariables(clazz);
        foreach (
            var localizationString in localizationStrings.Where(localization =>
                localization.Name != "title"
            )
        )
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

            var descriptionLocation = localizationString.Expression.GetLocation();
            foreach (
                var variableName in DynamicVariables.ParseReferencedVariables(
                    localizationString.Value
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
