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

    public static async Task<int> Discharge(
        PlayerChoiceContext choiceContext,
        Creature target,
        int maximum,
        Creature? applier,
        CardModel? cardSource
    )
    {
        if (CombatManager.Instance.IsOverOrEnding || maximum <= 0)
            return 0;
        if (target.GetPower<QiChargePower>() is not { } qiChargePower)
            return 0;

        var toConsume = Math.Min(maximum, qiChargePower.Amount);
        if (toConsume <= 0)
            return 0;

        await PowerCmd.ModifyAmount(choiceContext, qiChargePower, -toConsume, applier, cardSource);
        return toConsume;
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
