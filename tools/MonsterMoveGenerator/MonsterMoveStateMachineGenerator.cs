using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MonsterMoveGenerator;

[Generator]
public sealed class MonsterMoveStateMachineGenerator : IIncrementalGenerator
{
    private const string MonsterTypeName = "NewKunlun.NewKunlunCode.Monsters.NewKunlunMonster";
    private const string MonsterAttributeName = "NewKunlun.NewKunlunCode.Monsters.MonsterAttribute";
    private const string InitialStateAttributeName =
        "NewKunlun.NewKunlunCode.Monsters.InitialStateAttribute";
    private const string MoveStateTypeName =
        "MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine.MoveState";
    private const string CreatureTypeName = "MegaCrit.Sts2.Core.Entities.Creatures.Creature";
    private const string AbstractIntentTypeName =
        "MegaCrit.Sts2.Core.MonsterMoves.Intents.AbstractIntent";

    private static readonly DiagnosticDescriptor InvalidBaseClass = new(
        "NKMOVE001",
        "Monster class must inherit its generated base",
        "[Monster] class '{0}' must inherit from its generated '{0}Base' class",
        "CodeGeneration",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    private static readonly DiagnosticDescriptor MissingIntent = new(
        "NKMOVE002",
        "Move intent property is missing",
        "Move method '{0}' requires a readable property named '{0}Intent'",
        "CodeGeneration",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    private static readonly DiagnosticDescriptor InvalidIntent = new(
        "NKMOVE003",
        "Move intent property has the wrong type",
        "Property '{0}Intent' must be implicitly convertible to IEnumerable<AbstractIntent>",
        "CodeGeneration",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    private static readonly DiagnosticDescriptor MissingInitialState = new(
        "NKMOVE004",
        "Initial move is missing",
        "Monster class '{0}' must mark at least one move method with [InitialState]",
        "CodeGeneration",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    private static readonly DiagnosticDescriptor InvalidInitialState = new(
        "NKMOVE005",
        "InitialState target is not a move method",
        "[InitialState] method '{0}' must have the signature Task<MoveState> {0}(IReadOnlyList<Creature> creatures)",
        "CodeGeneration",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    private static readonly DiagnosticDescriptor InvalidSingleWeight = new(
        "NKMOVE006",
        "A single initial move cannot be weighted",
        "Initial move '{0}' cannot specify a weight unless the monster has multiple initial moves",
        "CodeGeneration",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classes = context
            .SyntaxProvider.CreateSyntaxProvider(
                static (node, _) => node is ClassDeclarationSyntax { BaseList: not null },
                static (syntaxContext, _) => (ClassDeclarationSyntax)syntaxContext.Node
            )
            .Collect();

        context.RegisterSourceOutput(
            context.CompilationProvider.Combine(classes),
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
        var moveStateType = compilation.GetTypeByMetadataName(MoveStateTypeName);
        var creatureType = compilation.GetTypeByMetadataName(CreatureTypeName);
        var abstractIntentType = compilation.GetTypeByMetadataName(AbstractIntentTypeName);
        var taskType = compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1");
        var readOnlyListType = compilation.GetTypeByMetadataName(
            "System.Collections.Generic.IReadOnlyList`1"
        );
        var enumerableType = compilation.GetTypeByMetadataName(
            "System.Collections.Generic.IEnumerable`1"
        );
        if (
            moveStateType is null
            || creatureType is null
            || abstractIntentType is null
            || taskType is null
            || readOnlyListType is null
            || enumerableType is null
        )
            return;

        var intentEnumerableType = enumerableType.Construct(abstractIntentType);
        HashSet<INamedTypeSymbol> processed = new(SymbolEqualityComparer.Default);
        foreach (var candidate in candidates)
        {
            var semanticModel = compilation.GetSemanticModel(candidate.SyntaxTree);
            if (semanticModel.GetDeclaredSymbol(candidate) is not { } classSymbol)
                continue;
            if (!processed.Add(classSymbol) || !HasMonsterAttribute(classSymbol))
                continue;
            if (
                classSymbol.IsAbstract
                || classSymbol
                    .GetMembers("GenerateMoveStateMachine")
                    .OfType<IMethodSymbol>()
                    .Any(method => !method.IsImplicitlyDeclared)
            )
                continue;

            if (!UsesGeneratedBase(candidate, classSymbol))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        InvalidBaseClass,
                        candidate.Identifier.GetLocation(),
                        classSymbol.Name
                    )
                );
            }

            GenerateForClass(
                context,
                compilation,
                classSymbol,
                moveStateType,
                creatureType,
                taskType,
                readOnlyListType,
                intentEnumerableType
            );
        }
    }

    private static void GenerateForClass(
        SourceProductionContext context,
        Compilation compilation,
        INamedTypeSymbol classSymbol,
        INamedTypeSymbol moveStateType,
        INamedTypeSymbol creatureType,
        INamedTypeSymbol taskType,
        INamedTypeSymbol readOnlyListType,
        INamedTypeSymbol intentEnumerableType
    )
    {
        var declaration = classSymbol
            .DeclaringSyntaxReferences.Select(reference => reference.GetSyntax())
            .OfType<ClassDeclarationSyntax>()
            .FirstOrDefault();
        if (declaration is null)
            return;

        var methods = classSymbol.GetMembers().OfType<IMethodSymbol>().ToArray();
        var moveMethods = methods
            .Where(method =>
                IsMoveMethod(method, moveStateType, creatureType, taskType, readOnlyListType)
            )
            .OrderBy(method => method.Locations.FirstOrDefault()?.SourceSpan.Start ?? int.MaxValue)
            .ToArray();
        var annotatedMethods = methods
            .Select(method => new
            {
                Method = method,
                Attribute = method
                    .GetAttributes()
                    .FirstOrDefault(attribute =>
                        attribute.AttributeClass?.ToDisplayString() == InitialStateAttributeName
                    ),
            })
            .Where(item => item.Attribute is not null)
            .ToArray();

        foreach (var item in annotatedMethods)
        {
            if (!moveMethods.Contains(item.Method, SymbolEqualityComparer.Default))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        InvalidInitialState,
                        item.Method.Locations.FirstOrDefault(),
                        item.Method.Name
                    )
                );
            }
        }

        if (annotatedMethods.Length == 0)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    MissingInitialState,
                    declaration.Identifier.GetLocation(),
                    classSymbol.Name
                )
            );
        }

        List<MoveDefinition> moves = [];
        foreach (var method in moveMethods)
        {
            var initialAttribute = method
                .GetAttributes()
                .FirstOrDefault(attribute =>
                    attribute.AttributeClass?.ToDisplayString() == InitialStateAttributeName
                );
            var weight = GetWeight(initialAttribute);
            moves.Add(new MoveDefinition(method.Name, initialAttribute is not null, weight));

            var intentName = method.Name + "Intent";
            var intent = classSymbol
                .GetMembers(intentName)
                .OfType<IPropertySymbol>()
                .FirstOrDefault();
            if (intent?.GetMethod is null)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(MissingIntent, method.Locations.FirstOrDefault(), method.Name)
                );
                continue;
            }

            if (!compilation.ClassifyConversion(intent.Type, intentEnumerableType).IsImplicit)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(InvalidIntent, method.Locations.FirstOrDefault(), method.Name)
                );
            }
        }

        var initialMoves = moves.Where(move => move.IsInitial).ToArray();
        if (initialMoves.Length == 1 && Math.Abs(initialMoves[0].Weight - 1D) > double.Epsilon)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    InvalidSingleWeight,
                    moveMethods
                        .First(method => method.Name == initialMoves[0].Name)
                        .Locations.FirstOrDefault(),
                    initialMoves[0].Name
                )
            );
        }

        var source = Render(classSymbol, moves, initialMoves);
        context.AddSource(
            $"{SanitizeHintName(classSymbol.ToDisplayString())}.MonsterMoves.g.cs",
            SourceText.From(source, Encoding.UTF8)
        );
    }

    private static bool IsMoveMethod(
        IMethodSymbol method,
        INamedTypeSymbol moveStateType,
        INamedTypeSymbol creatureType,
        INamedTypeSymbol taskType,
        INamedTypeSymbol readOnlyListType
    )
    {
        if (
            method.MethodKind != MethodKind.Ordinary
            || method.IsStatic
            || method.Parameters.Length != 1
            || method.ReturnType is not INamedTypeSymbol returnType
            || !SymbolEqualityComparer.Default.Equals(returnType.OriginalDefinition, taskType)
            || returnType.TypeArguments.Length != 1
            || !SymbolEqualityComparer.Default.Equals(returnType.TypeArguments[0], moveStateType)
        )
            return false;

        return method.Parameters[0].Type is INamedTypeSymbol parameterType
            && SymbolEqualityComparer.Default.Equals(
                parameterType.OriginalDefinition,
                readOnlyListType
            )
            && parameterType.TypeArguments.Length == 1
            && SymbolEqualityComparer.Default.Equals(parameterType.TypeArguments[0], creatureType);
    }

    private static bool UsesGeneratedBase(
        ClassDeclarationSyntax declaration,
        INamedTypeSymbol classSymbol
    )
    {
        var expectedName = classSymbol.Name + "Base";
        return declaration.BaseList?.Types.Any(type =>
                type.Type switch
                {
                    IdentifierNameSyntax identifier => identifier.Identifier.ValueText,
                    QualifiedNameSyntax qualified => qualified.Right.Identifier.ValueText,
                    AliasQualifiedNameSyntax alias => alias.Name.Identifier.ValueText,
                    _ => string.Empty,
                } == expectedName
            ) == true;
    }

    private static bool HasMonsterAttribute(INamedTypeSymbol classSymbol) =>
        classSymbol
            .GetAttributes()
            .Any(attribute => attribute.AttributeClass?.ToDisplayString() == MonsterAttributeName);

    private static double GetWeight(AttributeData? attribute)
    {
        if (attribute is null || attribute.ConstructorArguments.Length == 0)
            return 1D;
        return attribute.ConstructorArguments[0].Value is double weight ? weight : 1D;
    }

    private static string Render(
        INamedTypeSymbol classSymbol,
        IReadOnlyList<MoveDefinition> moves,
        IReadOnlyList<MoveDefinition> initialMoves
    )
    {
        var source = new StringBuilder("// <auto-generated />\n#nullable enable\n");
        if (!classSymbol.ContainingNamespace.IsGlobalNamespace)
            source
                .Append("namespace ")
                .Append(classSymbol.ContainingNamespace.ToDisplayString())
                .Append(";\n\n");

        source
            .Append(AccessibilityText(classSymbol.DeclaredAccessibility))
            .Append(" abstract class ")
            .Append(classSymbol.Name)
            .Append("Base : global::")
            .Append(MonsterTypeName)
            .Append("\n{\n");
        foreach (var move in moves)
        {
            source
                .Append(
                    "    protected abstract global::System.Collections.Generic.IEnumerable<global::MegaCrit.Sts2.Core.MonsterMoves.Intents.AbstractIntent> "
                )
                .Append(move.Name)
                .Append("Intent { get; }\n\n")
                .Append(
                    "    protected abstract global::System.Threading.Tasks.Task<global::MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine.MoveState> "
                )
                .Append(move.Name)
                .Append(
                    "(global::System.Collections.Generic.IReadOnlyList<global::MegaCrit.Sts2.Core.Entities.Creatures.Creature> creatures);\n\n"
                )
                .Append(
                    "    private global::MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine.MoveState? __"
                )
                .Append(move.Name)
                .Append("Move;\n\n")
                .Append(
                    "    protected global::MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine.MoveState "
                )
                .Append(move.Name)
                .Append("Move => __")
                .Append(move.Name)
                .Append("Move ??= Create")
                .Append(move.Name)
                .Append("Move();\n\n")
                .Append(
                    "    private global::MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine.MoveState Create"
                )
                .Append(move.Name)
                .Append("Move()\n    {\n")
                .Append(
                    "        async global::System.Threading.Tasks.Task OnPerform(global::System.Collections.Generic.IReadOnlyList<global::MegaCrit.Sts2.Core.Entities.Creatures.Creature> creatures)\n"
                )
                .Append("        {\n            ")
                .Append(move.Name)
                .Append("Move.FollowUpState = await ")
                .Append(move.Name)
                .Append("(creatures);\n        }\n\n")
                .Append("        return new(")
                .Append(SymbolDisplay.FormatLiteral(move.Name, quote: true))
                .Append(", OnPerform, [.. ")
                .Append(move.Name)
                .Append("Intent]);\n    }\n\n");
        }

        source
            .Append(
                "    protected override global::MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine.MonsterMoveStateMachine GenerateMoveStateMachine()\n"
            )
            .Append("    {\n");
        if (initialMoves.Count == 1)
        {
            source
                .Append("        var initialState = ")
                .Append(initialMoves[0].Name)
                .Append("Move;\n");
        }
        else
        {
            source.Append(
                "        var initialState = new global::MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine.RandomBranchState(\"__GeneratedInitialState\");\n"
            );
            foreach (var move in initialMoves)
            {
                source
                    .Append("        initialState.AddBranch(")
                    .Append(move.Name)
                    .Append(
                        "Move, global::MegaCrit.Sts2.Core.MonsterMoves.MoveRepeatType.CanRepeatForever, "
                    )
                    .Append(((float)move.Weight).ToString("R", CultureInfo.InvariantCulture))
                    .Append("F);\n");
            }
        }

        source.Append("        return new([\n");
        foreach (var move in moves)
            source.Append("            ").Append(move.Name).Append("Move,\n");
        if (initialMoves.Count > 1)
            source.Append("            initialState,\n");
        source.Append("        ], initialState);\n    }\n}\n");
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

    private static string AccessibilityText(Accessibility accessibility) =>
        accessibility == Accessibility.Public ? "public" : "internal";

    private sealed class MoveDefinition(string name, bool isInitial, double weight)
    {
        public string Name { get; } = name;
        public bool IsInitial { get; } = isInitial;
        public double Weight { get; } = weight;
    }
}
