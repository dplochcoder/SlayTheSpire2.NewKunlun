using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Qi Charges",
    description: "Energy used to accelerate damage.\nConsumed by cards with the [gold]Discharge[/gold] keyword.",
    smartDescription: "Energy used to accelerate damage.\nConsumed by cards with the [gold]Discharge[/gold] keyword.{LockedCount:cond:>0?\n{LockedCount} {LockedCount:plural:charge is|charges are} [gold]Locked[/gold] {LockedTurnsRemaining:cond:>1?for 2 turns|until next turn}.|}"
)]
public partial class QiChargePower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar(nameof(LockedCount), 0M), new DynamicVar(nameof(LockedTurnsRemaining), 0M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.Discharge()];

    public int Available(QiChargeCmd.Locked locked)
    {
        if (LockedCount.BaseValue == 0 || locked == QiChargeCmd.Locked.IncludeLocked)
            return Amount;
        else
            return Math.Max(0, Amount - LockedCount.IntValue);
    }

    public bool Lock(int toLock)
    {
        if (Amount < toLock)
            return false;

        if (LockedCount.BaseValue < toLock)
            LockedCount.BaseValue = toLock;
        if (LockedTurnsRemaining.BaseValue < 2)
            LockedTurnsRemaining.BaseValue = 2;
        return true;
    }

    public async Task<int> Discharge(
        PlayerChoiceContext choiceContext,
        int maximum,
        QiChargeCmd.Locked locked,
        CardModel? cardSource
    )
    {
        var available = Available(locked);
        var discharged = Math.Min(maximum, available);
        if (discharged <= 0)
            return 0;

        if (locked == QiChargeCmd.Locked.IncludeLocked)
        {
            var lockedDischarged = Math.Max(discharged, LockedCount.IntValue);
            LockedCount.BaseValue -= lockedDischarged;
            if (LockedCount.BaseValue == 0)
                LockedTurnsRemaining.BaseValue = 0;
        }

        await PowerCmd.ModifyAmount(choiceContext, this, -discharged, Owner, cardSource);
        return discharged;
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (!participants.Contains(Owner))
            return;

        if (LockedCount.BaseValue > 0 && --LockedTurnsRemaining.BaseValue <= 0)
        {
            await PowerCmd.ModifyAmount(choiceContext, this, -LockedCount.BaseValue, Owner, null);
            Flash();
        }
    }
}
