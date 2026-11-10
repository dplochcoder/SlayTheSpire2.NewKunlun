using MegaCrit.Sts2.Core.Entities.Creatures;

namespace NewKunlun.NewKunlunCode.Monsters.Intents;

public class SingleCrimsonAttackIntent : CrimsonAttackIntent
{
    public override int Repeats => 1;

    public SingleCrimsonAttackIntent(Func<decimal> damageCalc) => DamageCalc = damageCalc;

    public SingleCrimsonAttackIntent(decimal damage)
        : this(() => damage) { }

    public override int GetTotalDamage(IEnumerable<Creature> targets, Creature owner) =>
        GetSingleDamage(targets, owner);
}
