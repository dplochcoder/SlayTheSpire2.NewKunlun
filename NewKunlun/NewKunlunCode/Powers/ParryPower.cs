using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Parry",
    description: "Each time you block an attack this turn, lose 1 [gold]Parry[/gold] and gain 1 [gold]Qi Charge[/gold].\nIf you would full block, make a [gold]Precise Parry[/gold]."
)]
public class ParryPower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.QiCharge()];

    private int _parries = 0;
    private int _preciseParries = 0;

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource
    )
    {
        if (_parries == 0)
            return;

        var qiSwipeJade = Owner.GetPower<QiSwipeJadePower>();
        qiSwipeJade?.Flash();
        await QiChargeCmd.GainQiCharges(
            choiceContext,
            target,
            _parries * (1 + qiSwipeJade?.Amount ?? 0),
            Owner,
            null
        );
        await PowerCmd.ModifyAmount(choiceContext, this, -_parries, dealer, cardSource);

        if (_preciseParries > 0 && Owner.GetPower<DownloadPower>() is { } download)
        {
            await PowerCmd.Apply<StrengthPower>(
                choiceContext,
                target,
                download.Amount * _preciseParries,
                target,
                null
            );
            download.Flash();
        }

        _parries = 0;
        _preciseParries = 0;
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
        [HarmonyPrefix]
        private static bool Prefix(
            Creature __instance,
            decimal amount,
            ValueProp props,
            ref decimal __result
        )
        {
            if (!props.IsPoweredAttack() || props.HasFlag(ValueProp.Unblockable))
                return true;

            var self = __instance;
            if (
                self.GetPower<ParryPower>() is { } parryPower
                && parryPower._parries < parryPower.Amount
                && self.Block > 0
            )
            {
                parryPower._parries++;
                if (self.Block >= amount)
                {
                    parryPower._preciseParries++;
                    __result = amount;
                    return false;
                }
            }
            return true;
        }
    }
}
