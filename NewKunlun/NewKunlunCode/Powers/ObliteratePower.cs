using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Obliterate",
    description: "Your attacks deal double damage this turn.",
    smartDescription: "Your attacks deal {Amount}x damage this turn."
)]
public class ObliteratePower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay
    ) => dealer == Owner && props.IsPoweredAttack() ? amount * Amount : amount;

    public override Task AfterModifyingDamageAmount(CardModel? cardSource)
    {
        Flash();
        return Task.CompletedTask;
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (side == Owner.Side)
            return;

        await PowerCmd.Remove(this);
    }
}
