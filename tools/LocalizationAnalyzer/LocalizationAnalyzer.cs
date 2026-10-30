using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CardLocalizationAnalyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class LocalizationAnalyzer : DiagnosticAnalyzer
{
    private static readonly List<DiagnosticDescriptor> Descriptors = [];

    private static DiagnosticDescriptor RegisterDescriptor(DiagnosticDescriptor descriptor)
    {
        Descriptors.Add(descriptor);
        return descriptor;
    }

    private static readonly DiagnosticDescriptor InvalidLocalizationDescriptor = RegisterDescriptor(
        new DiagnosticDescriptor(
            "NKLOC001",
            "Invalid model localization",
            "Localization fields must be string literals",
            "Localization",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Localization is generated at build time and requires constant string literals."
        )
    );

    private static Diagnostic InvalidLocalization(Location location) =>
        Diagnostic.Create(InvalidLocalizationDescriptor, location);

    private static readonly DiagnosticDescriptor UnknownVariableDescriptor = RegisterDescriptor(
        new DiagnosticDescriptor(
            "NKLOC002",
            "Unknown model description variable",
            "Description for '{0}' references unknown dynamic variable '{1}'; valid names: {2}{3}",
            "Localization",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Variables referenced by a model description must be declared in CanonicalVars."
        )
    );

    private static Diagnostic UnknownVariable(
        Location location,
        string className,
        string variableName,
        string validNames,
        string suggestion
    ) =>
        Diagnostic.Create(
            UnknownVariableDescriptor,
            location,
            className,
            variableName,
            validNames,
            suggestion
        );

    private static readonly DiagnosticDescriptor UnnamedArgumentDescriptor = RegisterDescriptor(
        new DiagnosticDescriptor(
            "NKLOC003",
            "Localization argument must be named",
            "Localization arguments must use named-argument syntax",
            "Localization",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Named arguments keep localization fields independent of constructor parameter order."
        )
    );

    private static Diagnostic UnnamedArgument(Location location) =>
        Diagnostic.Create(UnnamedArgumentDescriptor, location);

    private static readonly DiagnosticDescriptor MissingOrMismatchedLocalizationDescriptor =
        RegisterDescriptor(
            new DiagnosticDescriptor(
                "NKLOC004",
                "Missing or mismatched localization attribute",
                "'{0}' derives from {1} and must use [{2}] localization; {3}",
                "Localization",
                DiagnosticSeverity.Error,
                isEnabledByDefault: true,
                description: "Every localized model must declare the localization attribute matching its model hierarchy."
            )
        );

    private static Diagnostic MissingOrMismatchedLocalization(
        Location location,
        string className,
        string modelName,
        string expectedAttributeName,
        string details
    ) =>
        Diagnostic.Create(
            MissingOrMismatchedLocalizationDescriptor,
            location,
            className,
            modelName,
            expectedAttributeName,
            details
        );

    private static readonly DiagnosticDescriptor InvalidKeywordLocalizationTargetDescriptor =
        RegisterDescriptor(
            new DiagnosticDescriptor(
                "NKLOC005",
                "Invalid keyword localization target",
                "[KeywordLocalization] may only be applied to static CardKeyword fields",
                "Localization",
                DiagnosticSeverity.Error,
                isEnabledByDefault: true
            )
        );

    private static Diagnostic InvalidKeywordLocalizationTarget(Location location) =>
        Diagnostic.Create(InvalidKeywordLocalizationTargetDescriptor, location);

    private static readonly DiagnosticDescriptor InvalidStaticHoverTipLocalizationTargetDescriptor =
        RegisterDescriptor(
            new DiagnosticDescriptor(
                "NKLOC006",
                "Invalid static hover-tip localization target",
                "[StaticHoverTipLocalization] may only be applied to static StaticHoverTip fields declared with [CustomEnum]",
                "Localization",
                DiagnosticSeverity.Error,
                isEnabledByDefault: true
            )
        );

    private static Diagnostic InvalidStaticHoverTipLocalizationTarget(Location location) =>
        Diagnostic.Create(InvalidStaticHoverTipLocalizationTargetDescriptor, location);

    private static readonly DiagnosticDescriptor LocalizationAttributeOnAbstractClassDescriptor =
        RegisterDescriptor(
            new DiagnosticDescriptor(
                "NKLOC007",
                "Localization attribute on abstract class",
                "Localization attributes should not be placed on abstract classes",
                "Localization",
                DiagnosticSeverity.Error,
                isEnabledByDefault: true,
                description: "Localization attributes should only be placed on concrete classes."
            )
        );

    private static Diagnostic LocalizationAttributeOnAbstractClass(Location location) =>
        Diagnostic.Create(LocalizationAttributeOnAbstractClassDescriptor, location);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [.. Descriptors];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeClass, SyntaxKind.ClassDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeField, SyntaxKind.FieldDeclaration);
    }

    private static void AnalyzeField(SyntaxNodeAnalysisContext context)
    {
        var field = (FieldDeclarationSyntax)context.Node;
        var attributes = field.AttributeLists.SelectMany(list => list.Attributes).ToArray();
        var attribute = attributes.FirstOrDefault(candidate =>
            IsNamedAttribute(candidate, "KeywordLocalization")
            || IsNamedAttribute(candidate, "StaticHoverTipLocalization")
        );
        if (attribute is null)
            return;

        var isStatic = field.Modifiers.Any(SyntaxKind.StaticKeyword);
        var fieldTypeName = context.SemanticModel.GetTypeInfo(field.Declaration.Type).Type?.Name;
        var isStaticHoverTip = IsNamedAttribute(attribute, "StaticHoverTipLocalization");
        var validTarget = isStaticHoverTip
            ? isStatic
                && fieldTypeName == "StaticHoverTip"
                && attributes.Any(candidate => IsNamedAttribute(candidate, "CustomEnum"))
            : isStatic && fieldTypeName == "CardKeyword";
        if (!validTarget)
            context.ReportDiagnostic(
                isStaticHoverTip
                    ? InvalidStaticHoverTipLocalizationTarget(attribute.GetLocation())
                    : InvalidKeywordLocalizationTarget(attribute.GetLocation())
            );

        var arguments = attribute.ArgumentList?.Arguments ?? default;
        foreach (var argument in arguments.Where(argument => argument.NameColon is null))
            context.ReportDiagnostic(UnnamedArgument(argument.Expression.GetLocation()));

        var namedArguments = arguments.Where(argument => argument.NameColon is not null).ToArray();
        var title = namedArguments.FirstOrDefault(argument =>
            argument.NameColon!.Name.Identifier.ValueText == "title"
        );
        var description = namedArguments.FirstOrDefault(argument =>
            argument.NameColon!.Name.Identifier.ValueText == "description"
        );
        if (
            namedArguments.Length != 2
            || title is null
            || description is null
            || !IsStringLiteral(title.Expression)
            || !IsStringLiteral(description.Expression)
        )
            context.ReportDiagnostic(InvalidLocalization(attribute.GetLocation()));
    }

    private static bool IsStringLiteral(ExpressionSyntax expression) =>
        expression is LiteralExpressionSyntax literal
        && literal.IsKind(SyntaxKind.StringLiteralExpression);

    private static bool IsNamedAttribute(AttributeSyntax attribute, string name) =>
        attribute.Name.ToString() == name
        || attribute.Name.ToString() == $"{name}Attribute"
        || attribute.Name.ToString().EndsWith($".{name}")
        || attribute.Name.ToString().EndsWith($".{name}Attribute");

    private static void AnalyzeClass(SyntaxNodeAnalysisContext context)
    {
        var clazz = (ClassDeclarationSyntax)context.Node;
        if (context.SemanticModel.GetDeclaredSymbol(clazz) is not { } classSymbol)
            return;
        if (!Localization.TryGetModelKind(classSymbol, out var kind))
            return;

        var expectedAttributeName = Localization.GetAttributeName(kind);
        var localizationAttributes = Localization.FindLocalizationAttributes(clazz).ToArray();
        if (classSymbol.IsAbstract)
        {
            if (localizationAttributes.Length > 0)
            {
                context.ReportDiagnostic(
                    LocalizationAttributeOnAbstractClass(localizationAttributes[0].GetLocation())
                );
            }
            return;
        }

        var attr = Localization.FindLocalizationAttribute(clazz, kind);
        foreach (
            var mismatchedAttribute in localizationAttributes.Where(candidate => candidate != attr)
        )
        {
            context.ReportDiagnostic(
                MissingOrMismatchedLocalization(
                    mismatchedAttribute.GetLocation(),
                    className: classSymbol.Name,
                    modelName: Localization.GetModelName(kind),
                    expectedAttributeName: expectedAttributeName,
                    details: $"found [{mismatchedAttribute.Name}]"
                )
            );
        }

        if (attr is null)
        {
            if (localizationAttributes.Length == 0)
                context.ReportDiagnostic(
                    MissingOrMismatchedLocalization(
                        clazz.Identifier.GetLocation(),
                        className: classSymbol.Name,
                        modelName: Localization.GetModelName(kind),
                        expectedAttributeName: expectedAttributeName,
                        details: "no localization attribute was found"
                    )
                );
            return;
        }

        var unnamedArguments =
            attr.ArgumentList?.Arguments.Where(argument => argument.NameColon is null).ToArray()
            ?? [];
        foreach (var argument in unnamedArguments)
            context.ReportDiagnostic(UnnamedArgument(argument.Expression.GetLocation()));
        if (unnamedArguments.Length > 0)
            return;

        if (
            !Localization.GetLocalizationStrings(
                attr,
                kind,
                out var localizationStrings,
                out var skipValidation
            )
        )
        {
            context.ReportDiagnostic(InvalidLocalization(attr.GetLocation()));
            return;
        }

        if (skipValidation)
            return;

        var validVariables = DynamicVariables.FindDynamicVariables(clazz, context.SemanticModel);
        foreach (
            var localizationString in localizationStrings.Where(localization =>
                localization.Name != "title"
            )
        )
        {
            HashSet<string> allowedVariables = [.. validVariables];
            switch (kind)
            {
                case LocalizedModelKind.Affliction:
                case LocalizedModelKind.Enchantment:
                case LocalizedModelKind.Power:
                    allowedVariables.Add("Amount");
                    break;
                case LocalizedModelKind.Card:
                    allowedVariables.Add("IfUpgraded");
                    break;
                case LocalizedModelKind.Potion:
                case LocalizedModelKind.Relic:
                    break;
                default:
                    throw new NotSupportedException($"{kind}");
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
                    UnknownVariable(
                        descriptionLocation,
                        className: clazz.Identifier.ValueText,
                        variableName: variableName,
                        validNames: validNames,
                        suggestion: suggestion
                    )
                );
            }
        }
    }
}
