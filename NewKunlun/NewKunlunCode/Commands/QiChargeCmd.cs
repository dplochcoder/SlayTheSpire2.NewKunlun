using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Powers;

namespace NewKunlun.NewKunlunCode.Commands;

public static class QiChargeCmd
{
    private const int DefaultMaxCharges = 5;

    public static int GetCapacity(Creature target) =>
        target.GetPower<QiChargeCapacityPower>()?.Amount ?? DefaultMaxCharges;

    public static async Task GainQiCharges(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount,
        Creature? applier,
        CardModel? cardSource
    )
    {
        if (CombatManager.Instance.IsOverOrEnding || amount <= 0)
            return;

        var curValue = target.GetPowerAmount<QiChargePower>();
        var newValue = Math.Min(
            target.GetPowerAmount<QiChargePower>() + amount,
            GetCapacity(target)
        );
        if (newValue > curValue)
            await PowerCmd.Apply<QiChargePower>(
                choiceContext,
                target,
                newValue - curValue,
                applier,
                cardSource
            );
        else
            target.GetPower<QiChargePower>()?.Flash();
    }

    public enum Locked
    {
        IncludeLocked,
        ExcludeLocked,
    }

    public static int GetAvailable(Creature target, Locked locked) =>
        target.GetPower<QiChargePower>()?.Available(locked) ?? 0;

    public static bool Lock(Creature target, int toLock) =>
        target.GetPower<QiChargePower>()?.Lock(toLock) ?? false;

    public static async Task<int> Discharge(
        PlayerChoiceContext choiceContext,
        Creature target,
        int maximum,
        Locked locked,
        Creature? applier,
        CardModel? cardSource
    )
    {
        if (CombatManager.Instance.IsOverOrEnding || maximum <= 0)
            return 0;
        if (target.GetPower<QiChargePower>() is not { } qiChargePower)
            return 0;

        return await qiChargePower.Discharge(choiceContext, maximum, locked, cardSource);
    }

    public static async Task IncreaseQiChargeCapacity(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount,
        Creature? applier,
        CardModel? cardSource
    )
    {
        if (target.GetPowerAmount<QiChargeCapacityPower>() == 0)
            amount += DefaultMaxCharges;

        await PowerCmd.Apply<QiChargeCapacityPower>(
            choiceContext,
            target,
            amount,
            applier,
            cardSource,
            silent: true
        );
    }
}
