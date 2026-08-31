using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Powers;

namespace NewKunlun.NewKunlunCode.Commands;

public static class InternalDamageCmd
{
    public static async Task Apply(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount,
        Creature? applier,
        CardModel? cardSource,
        bool silent = false
    )
    {
        if (amount <= 0)
            return;

        // TODO: Hook modifiers.
        await PowerCmd.Apply<InternalDamagePower>(
            choiceContext,
            target,
            amount,
            applier,
            cardSource,
            silent
        );
    }

    // Returns the amount of internal damage healed.
    public static async Task<decimal> Heal(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal maxAmount,
        Creature? applier,
        CardModel? cardSource,
        bool silent = false
    )
    {
        if (target.GetPower<InternalDamagePower>() is not { } power)
            return 0;

        var toHeal = Math.Min(power.Amount, maxAmount);
        if (toHeal <= 0)
            return 0;

        await PowerCmd.ModifyAmount(choiceContext, power, -toHeal, applier, cardSource, silent);
        return toHeal;
    }
}
