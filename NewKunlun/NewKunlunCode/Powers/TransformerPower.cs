using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Transformer",
    description: "At the end of your turn, transform leftover {1:energyIcons()} into {Amount:plural:|{Amount} }[gold]Qi Charges[/gold]{Amount:plural:| each}."
)]
public class TransformerPower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (
            !participants.Contains(Owner)
            || Owner.Player is not { } player
            || player.PlayerCombatState == null
        )
            return;

        var space =
            QiChargeCmd.GetCapacity(Owner)
            - QiChargeCmd.GetAvailable(Owner, QiChargeCmd.Locked.IncludeLocked);
        var energy = player.PlayerCombatState.Energy;
        var toConsume = Math.Min(energy, space + (Amount - 1) / Amount);
        if (toConsume <= 0)
            return;

        await PlayerCmd.LoseEnergy(toConsume, player);
        await QiChargeCmd.GainQiCharges(choiceContext, Owner, toConsume * Amount, Owner, null);
        Flash();
    }
}
