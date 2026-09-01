# SlayTheSpire2.NewKunlun
A Slay the Spire 2 world and character mod inspired by Nine Sols content

## Card localization

Put each English card title and description on its `CardModel` class:

```csharp
[CardLocalization(
    title: "Example Card",
    description: "Deal {Damage:diff()} damage and gain {Block} block."
)]
public partial class ExampleCard : NewKunlunCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(8, ValueProp.Move), new BlockVar(5, ValueProp.Move)];
}
```

Placeholder names inside `{...}` must match a dynamic variable declared by the card. A typo
fails the build with error `NKLOC002` and, when possible, a suggested correction. Before
compilation, `LocalizationTool` updates `cards.json`, `powers.json`, and `relics.json` from the
corresponding localization attributes.

Named variables use `nameof(GeneratedProperty)`, which is shared by localization linting and
the generated strongly typed getter.

## Generated card variables

Card classes that declare `CanonicalVars` are partial. A source generator creates a typed
property for each canonical variable, so gameplay code does not need string keys or casts:

```csharp
protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new InternalDamageVar(nameof(EndTurnDamage), 4M),
        new DynamicVar(nameof(DamageIncrement), 4M),
    ];

// Generated in the other part of the partial class:
public InternalDamageVar EndTurnDamage =>
    (InternalDamageVar)DynamicVars["EndTurnDamage"];
public DynamicVar DamageIncrement => DynamicVars["DamageIncrement"];
```

Unnamed specialized variables use their type name (`DamageVar` becomes `Damage`, `BlockVar`
becomes `Block`). A base `DynamicVar` must be named with `nameof(PropertyName)`. `NKVAR001`
reports a card that is not partial, and `NKVAR002` reports an unnamed base variable.
