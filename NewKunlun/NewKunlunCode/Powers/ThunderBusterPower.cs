using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Thunder Buster",
    description: "",
    smartDescription: "At the end of your next {Amount:plural:turn|{Amount} turns}, deal {Damage} {HitCount} times to all enemies."
)]
public partial class ThunderBusterPower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(0M, ValueProp.Unpowered), new DynamicVar(nameof(HitCount), 0M)];

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (!participants.Contains(Owner))
            return;

        Flash();
        for (var i = 0; i < HitCount.BaseValue; i++)
            await CreatureCmd.Damage(
                choiceContext,
                Owner.CombatState?.HittableEnemies ?? [],
                Damage,
                Owner
            );
        await PowerCmd.Decrement(this);
    }
}
