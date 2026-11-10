using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using CSharpier.Core;
using CSharpier.Core.CSharp;
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
    private const string InitialMoveAttributeName =
        "NewKunlun.NewKunlunCode.Monsters.InitialMoveAttribute";
    private const string MoveAttributeName = "NewKunlun.NewKunlunCode.Monsters.MoveAttribute";
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

    private static readonly DiagnosticDescriptor MissingInitialMove = new(
        "NKMOVE002",
        "Initial move is missing",
        "Monster class '{0}' must mark at least one move method with [InitialMove]",
        "CodeGeneration",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    private static readonly DiagnosticDescriptor InvalidSingleWeight = new(
        "NKMOVE004",
        "A single initial move cannot be weighted",
        "Initial move '{0}' cannot specify a weight unless the monster has multiple initial moves",
        "CodeGeneration",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    private static readonly DiagnosticDescriptor MissingMoves = new(
        "NKMOVE005",
        "Move state machine has no moves",
        "Monster class '{0}' must declare at least one valid [Move] or [InitialMove] method",
        "CodeGeneration",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    private static readonly DiagnosticDescriptor InvalidMove = new(
        "NKMOVE006",
        "Move method has an invalid signature",
        "[Move] and [InitialMove] methods must return Task<MoveState>, accept only booleans or enums followed by a final IReadOnlyList<Creature> parameter",
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
                classSymbol,
                moveStateType,
                creatureType,
                taskType,
                readOnlyListType
            );
        }
    }

    private static void GenerateForClass(
        SourceProductionContext context,
        INamedTypeSymbol classSymbol,
        INamedTypeSymbol moveStateType,
        INamedTypeSymbol creatureType,
        INamedTypeSymbol taskType,
        INamedTypeSymbol readOnlyListType
    )
    {
        var declaration = classSymbol
            .DeclaringSyntaxReferences.Select(reference => reference.GetSyntax())
            .OfType<ClassDeclarationSyntax>()
            .FirstOrDefault();
        if (declaration is null)
            return;

        var methods = classSymbol.GetMembers().OfType<IMethodSymbol>().ToArray();
        var moveCandidates = methods.Where(HasMoveAttribute).ToArray();
        var moveMethods = moveCandidates
            .Where(method =>
                IsMoveMethod(method, moveStateType, creatureType, taskType, readOnlyListType)
            )
            .OrderBy(method => method.Locations.FirstOrDefault()?.SourceSpan.Start ?? int.MaxValue)
            .ToArray();

        if (moveMethods.Length == 0)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    MissingMoves,
                    declaration.Identifier.GetLocation(),
                    classSymbol.Name
                )
            );
        }

        foreach (
            var method in moveCandidates.Where(method =>
                !moveMethods.Contains(method, SymbolEqualityComparer.Default)
            )
        )
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    InvalidMove,
                    method.Locations.FirstOrDefault() ?? Location.None,
                    method.Name
                )
            );
        }

        if (!moveMethods.Any(HasInitialMoveAttribute))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    MissingInitialMove,
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
                    attribute.AttributeClass?.ToDisplayString() == InitialMoveAttributeName
                );
            var weight = GetWeight(initialAttribute);
            moves.Add(
                new MoveDefinition(
                    method.Name,
                    initialAttribute is not null,
                    weight,
                    method
                        .Parameters.Take(method.Parameters.Length - 1)
                        .Select(parameter => new MoveParameter(
                            EscapeIdentifier(parameter.Name),
                            parameter.Type.ToDisplayString(
                                SymbolDisplayFormat.FullyQualifiedFormat
                            ),
                            parameter.Type.SpecialType == SpecialType.System_Boolean,
                            parameter.Type.SpecialType == SpecialType.System_Boolean
                                ? 2
                                : parameter
                                    .Type.GetMembers()
                                    .OfType<IFieldSymbol>()
                                    .Where(field => field.HasConstantValue)
                                    .Select(field => field.ConstantValue)
                                    .Distinct()
                                    .Count()
                        ))
                        .ToArray()
                )
            );
        }

        var initialMoves = moves.Where(move => move.IsInitial).ToArray();
        var initialStateCount = initialMoves.Sum(move => move.CombinationCount);
        if (initialStateCount == 1 && Math.Abs(initialMoves[0].Weight - 1D) > double.Epsilon)
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
        var formattedSource = CSharpFormatter
            .Format(
                source,
                new CodeFormatterOptions { IncludeGenerated = true, EndOfLine = EndOfLine.LF }
            )
            .Code;
        context.AddSource(
            $"{SanitizeHintName(classSymbol.ToDisplayString())}.MonsterMoves.g.cs",
            SourceText.From(formattedSource, Encoding.UTF8)
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
            || method.Parameters.Length < 1
            || method.Parameters.Any(parameter => parameter.RefKind != RefKind.None)
            || method.ReturnType is not INamedTypeSymbol returnType
            || !SymbolEqualityComparer.Default.Equals(returnType.OriginalDefinition, taskType)
            || returnType.TypeArguments.Length != 1
            || !SymbolEqualityComparer.Default.Equals(returnType.TypeArguments[0], moveStateType)
        )
            return false;

        return method.Parameters[method.Parameters.Length - 1].Type
                is INamedTypeSymbol parameterType
            && SymbolEqualityComparer.Default.Equals(
                parameterType.OriginalDefinition,
                readOnlyListType
            )
            && parameterType.TypeArguments.Length == 1
            && SymbolEqualityComparer.Default.Equals(parameterType.TypeArguments[0], creatureType)
            && method
                .Parameters.Take(method.Parameters.Length - 1)
                .All(parameter =>
                    parameter.Type.SpecialType == SpecialType.System_Boolean
                    || parameter.Type.TypeKind == TypeKind.Enum
                );
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

    private static bool HasMoveAttribute(IMethodSymbol method) =>
        method
            .GetAttributes()
            .Any(attribute =>
                attribute.AttributeClass?.ToDisplayString()
                    is MoveAttributeName
                        or InitialMoveAttributeName
            );

    private static bool HasInitialMoveAttribute(IMethodSymbol method) =>
        method
            .GetAttributes()
            .Any(attribute =>
                attribute.AttributeClass?.ToDisplayString() == InitialMoveAttributeName
            );

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
            RenderMove(source, move);

        source
            .Append(
                "    protected override global::MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine.MonsterMoveStateMachine GenerateMoveStateMachine()\n"
            )
            .Append("    {\n");
        foreach (var move in moves.Where(move => move.Parameters.Count > 0))
            source.Append("        Initialize").Append(move.Name).Append("Moves();\n");
        if (moves.Any(move => move.Parameters.Count > 0))
            source.Append('\n');

        if (initialMoves.Count == 1 && initialMoves[0].Parameters.Count == 0)
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
                if (move.Parameters.Count == 0)
                    AppendInitialBranch(source, move, move.Name + "Move", "        ");
                else
                {
                    source
                        .Append("        foreach (var move in __")
                        .Append(move.Name)
                        .Append("Moves.Values)\n");
                    AppendInitialBranch(source, move, "move", "            ");
                }
            }
        }

        source.Append("        return new([\n");
        foreach (var move in moves)
        {
            if (move.Parameters.Count == 0)
                source.Append("            ").Append(move.Name).Append("Move,\n");
            else
                source.Append("            .. __").Append(move.Name).Append("Moves.Values,\n");
        }
        if (initialMoves.Count > 1 || initialMoves.Any(move => move.Parameters.Count > 0))
            source.Append("            initialState,\n");
        source.Append("        ], initialState);\n    }\n}\n");
        return source.ToString();
    }

    private static void RenderMove(StringBuilder source, MoveDefinition move)
    {
        var arguments = string.Join(", ", move.Parameters.Select(parameter => parameter.Name));

        source.Append(
            "    protected abstract global::System.Collections.Generic.IEnumerable<global::MegaCrit.Sts2.Core.MonsterMoves.Intents.AbstractIntent> "
        );
        source.Append(move.Name).Append("Intent");
        if (move.Parameters.Count == 0)
            source.Append(" { get; }\n\n");
        else
            AppendParameterList(source, move.Parameters, includeTargets: false).Append(";\n\n");

        source
            .Append(
                "    protected abstract global::System.Threading.Tasks.Task<global::MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine.MoveState> "
            )
            .Append(move.Name);
        AppendParameterList(source, move.Parameters, includeTargets: true).Append(";\n\n");

        if (move.Parameters.Count == 0)
        {
            source
                .Append(
                    "    private global::MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine.MoveState? __"
                )
                .Append(move.Name)
                .Append("Move;\n\n")
                .Append(
                    "    public global::MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine.MoveState "
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
                    "        async global::System.Threading.Tasks.Task OnPerform(global::System.Collections.Generic.IReadOnlyList<global::MegaCrit.Sts2.Core.Entities.Creatures.Creature> targets)\n"
                )
                .Append("        {\n            ")
                .Append(move.Name)
                .Append("Move.FollowUpState = await ")
                .Append(move.Name)
                .Append("(targets);\n        }\n\n")
                .Append("        return new(")
                .Append(SymbolDisplay.FormatLiteral(move.Name, quote: true))
                .Append(", OnPerform, [.. ")
                .Append(move.Name)
                .Append("Intent]);\n    }\n\n");
            return;
        }

        source
            .Append("    private readonly global::System.Collections.Generic.Dictionary<")
            .Append(TupleType(move.Parameters))
            .Append(
                ", global::MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine.MoveState> __"
            )
            .Append(move.Name)
            .Append("Moves = [];\n\n")
            .Append(
                "    protected global::MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine.MoveState "
            )
            .Append(move.Name)
            .Append("Move");
        AppendParameterList(source, move.Parameters, includeTargets: false)
            .Append(" => __")
            .Append(move.Name)
            .Append("Moves[")
            .Append(TupleExpression(move.Parameters))
            .Append("];\n\n")
            .Append("    private void Initialize")
            .Append(move.Name)
            .Append("Moves()\n    {\n")
            .Append("        if (__")
            .Append(move.Name)
            .Append("Moves.Count > 0)\n            return;\n\n")
            .Append("        var moveIndex = 0;\n");

        foreach (var parameter in move.Parameters)
        {
            source.Append("        foreach (var ").Append(parameter.Name).Append(" in ");
            if (parameter.IsBoolean)
                source.Append("new[] { false, true }");
            else
                source
                    .Append(
                        "global::System.Linq.Enumerable.Distinct(global::System.Enum.GetValues<"
                    )
                    .Append(parameter.Type)
                    .Append(">())");
            source.Append(")\n");
        }

        var indent = new string(' ', 8 + move.Parameters.Count * 4);
        source
            .Append(indent)
            .Append("{\n")
            .Append(indent)
            .Append(
                "    async global::System.Threading.Tasks.Task OnPerform(global::System.Collections.Generic.IReadOnlyList<global::MegaCrit.Sts2.Core.Entities.Creatures.Creature> targets)\n"
            )
            .Append(indent)
            .Append("    {\n")
            .Append(indent)
            .Append("        ")
            .Append(move.Name)
            .Append("Move(")
            .Append(arguments)
            .Append(").FollowUpState = await ")
            .Append(move.Name)
            .Append('(')
            .Append(arguments)
            .Append(", targets);\n")
            .Append(indent)
            .Append("    }\n\n")
            .Append(indent)
            .Append("    __")
            .Append(move.Name)
            .Append("Moves.Add(")
            .Append(TupleExpression(move.Parameters))
            .Append(", new($\"")
            .Append(move.Name)
            .Append("_{moveIndex++}\", OnPerform, [.. ")
            .Append(move.Name)
            .Append("Intent(")
            .Append(arguments)
            .Append(")]));\n")
            .Append(indent)
            .Append("}\n")
            .Append("    }\n\n");
    }

    private static StringBuilder AppendParameterList(
        StringBuilder source,
        IReadOnlyList<MoveParameter> parameters,
        bool includeTargets
    )
    {
        source.Append('(');
        for (var index = 0; index < parameters.Count; index++)
        {
            if (index > 0)
                source.Append(", ");
            var parameter = parameters[index];
            source.Append(parameter.Type).Append(' ').Append(parameter.Name);
        }
        if (includeTargets)
        {
            if (parameters.Count > 0)
                source.Append(", ");
            source.Append(
                "global::System.Collections.Generic.IReadOnlyList<global::MegaCrit.Sts2.Core.Entities.Creatures.Creature> targets"
            );
        }
        return source.Append(')');
    }

    private static string TupleType(IReadOnlyList<MoveParameter> parameters) =>
        parameters.Count == 1
            ? $"global::System.ValueTuple<{parameters[0].Type}>"
            : $"({string.Join(", ", parameters.Select(parameter => $"{parameter.Type} {parameter.Name}"))})";

    private static string TupleExpression(IReadOnlyList<MoveParameter> parameters) =>
        parameters.Count == 1
            ? $"new global::System.ValueTuple<{parameters[0].Type}>({parameters[0].Name})"
            : $"({string.Join(", ", parameters.Select(parameter => parameter.Name))})";

    private static void AppendInitialBranch(
        StringBuilder source,
        MoveDefinition move,
        string stateExpression,
        string indent
    ) =>
        source
            .Append(indent)
            .Append("initialState.AddBranch(")
            .Append(stateExpression)
            .Append(", global::MegaCrit.Sts2.Core.MonsterMoves.MoveRepeatType.CanRepeatForever, ")
            .Append(((float)move.Weight).ToString("R", CultureInfo.InvariantCulture))
            .Append("F);\n");

    private static string SanitizeHintName(string name)
    {
        var result = new StringBuilder(name.Length);
        foreach (var character in name)
            result.Append(
                char.IsLetterOrDigit(character) || character is '.' or '_' ? character : '_'
            );
        return result.ToString();
    }

    private static string EscapeIdentifier(string name) =>
        SyntaxFacts.GetKeywordKind(name) == SyntaxKind.None ? name : "@" + name;

    private static string AccessibilityText(Accessibility accessibility) =>
        accessibility == Accessibility.Public ? "public" : "internal";

    private sealed class MoveDefinition(
        string name,
        bool isInitial,
        double weight,
        IReadOnlyList<MoveParameter> parameters
    )
    {
        public string Name { get; } = name;
        public bool IsInitial { get; } = isInitial;
        public double Weight { get; } = weight;
        public IReadOnlyList<MoveParameter> Parameters { get; } = parameters;
        public long CombinationCount { get; } =
            parameters.Aggregate(1L, (count, parameter) => count * parameter.ValueCount);
    }

    private sealed class MoveParameter(string name, string type, bool isBoolean, int valueCount)
    {
        public string Name { get; } = name;
        public string Type { get; } = type;
        public bool IsBoolean { get; } = isBoolean;
        public int ValueCount { get; } = valueCount;
    }
}
