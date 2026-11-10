using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using NewKunlun.NewKunlunCode.Localization;
using LocTable = NewKunlun.NewKunlunCode.Localization.LocTable;

namespace NewKunlun.NewKunlunCode.Monsters.Intents;

public abstract partial class CrimsonAttackIntent : AttackIntent
{
    [CustomLocString(
        LocTable.Intents,
        "The enemy intends to do a [gold]Crimson[/gold] attack for {Damage} damage{Repeat:cond:>1? {Repeat} times|}.\nUnblocked [gold]Crimson[/gold] damage is doubled."
    )]
    private static partial LocString CrimsonAttackString { get; }

    protected override LocString IntentLabelFormat => CrimsonAttackString;

    public override int GetTotalDamage(IEnumerable<Creature> targets, Creature owner) =>
        GetSingleDamage(targets, owner) * Repeats;

    public override LocString GetIntentLabel(IEnumerable<Creature> targets, Creature owner)
    {
        var label = IntentLabelFormat;
        label.Add("Damage", (decimal)(int)GetTotalDamage(targets, owner));
        label.Add("Repeat", Repeats);
        return label;
    }
}
