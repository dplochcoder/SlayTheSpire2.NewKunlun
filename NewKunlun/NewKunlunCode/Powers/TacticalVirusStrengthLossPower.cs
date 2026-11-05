using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Tactical Virus",
    description: "Lose [gold]Strength[/gold] this turn.",
    smartDescription: "Lose {Amount} [gold]Strength[/gold] this turn."
)]
public class TacticalVirusStrengthLossPower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource
    )
    {
        if (power != this)
            return;

        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, -amount, Owner, cardSource);
    }

    public override Task AfterRemoved(Creature oldOwner) =>
        PowerCmd.Apply<StrengthPower>(
            new ThrowingPlayerChoiceContext(),
            oldOwner,
            Amount,
            oldOwner,
            null
        );

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (side != Owner.Side)
            return;

        Flash();
        await PowerCmd.Remove(this);
    }
}
