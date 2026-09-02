using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Powers;

public static class QiChargeCmd
{
    private const int DefaultMaxCharges = 5;

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

        var max = target.GetPowerAmount<QiChargeCapacityPower>();
        if (max == 0)
        {
            await PowerCmd.Apply<QiChargeCapacityPower>(
                choiceContext,
                target,
                DefaultMaxCharges,
                applier,
                cardSource,
                silent: true
            );
            max = DefaultMaxCharges;
        }

        var curValue = target.GetPowerAmount<QiChargePower>();
        var newValue = Math.Min(target.GetPowerAmount<QiChargePower>() + amount, max);
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

    public static async Task<decimal> ConsumeQiCharges(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal maximum,
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
}
