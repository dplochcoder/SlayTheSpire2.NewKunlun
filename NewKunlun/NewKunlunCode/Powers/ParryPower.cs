using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Parry",
    description: "Each time you would full-block an attack this turn, lose 1 [gold]Parry[/gold] and gain 1 [gold]Qi Charge[/gold] instead of losing block."
)]
public class ParryPower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.QiCharge()];

    private int _successfulParries = 0;

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource
    )
    {
        if (_successfulParries == 0)
            return;

        // TODO: FX
        await QiChargeCmd.GainQiCharges(
            choiceContext,
            target,
            _successfulParries * (1 + Owner.GetPowerAmount<QiSwipeJadePower>()),
            Owner,
            null
        );
        await PowerCmd.ModifyAmount(choiceContext, this, -_successfulParries, dealer, cardSource);
        _successfulParries = 0;
        Flash();
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (!participants.Contains(Owner))
            return;

        await PowerCmd.Remove(this);
        Flash();
    }

    [HarmonyPatch]
    private static class Patches
    {
        [HarmonyPatch(typeof(Creature), nameof(Creature.DamageBlockInternal))]
        private static bool Prefix(
            Creature instance,
            decimal amount,
            ValueProp props,
            ref decimal result
        )
        {
            if (!props.IsPoweredAttack() || props.HasFlag(ValueProp.Unblockable))
                return true;

            if (
                instance.GetPower<ParryPower>() is { } parryPower
                && parryPower._successfulParries < parryPower.Amount
                && instance.Block >= amount
            )
            {
                parryPower._successfulParries++;
                result = amount;
                return false;
            }
            return true;
        }
    }
}
