using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Hooks;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Commands;

public static class InternalDamageCmd
{
    public static async Task Inflict(
        PlayerChoiceContext choiceContext,
        Creature target,
        InternalDamageInflictVar inflict,
        Creature? applier,
        CardModel? cardSource,
        bool silent = false
    )
    {
        var amount = IInternalDamageListener.ModifyInternalDamageInflicted(
            target.CombatState!,
            target,
            inflict.BaseValue,
            applier,
            cardSource
        );
        if (amount <= 0)
            return;

        await PowerCmd.Apply<InternalDamagePower>(
            choiceContext,
            target,
            amount,
            applier,
            cardSource,
            silent
        );
        await IInternalDamageListener.InvokeInternalDamageTaken(
            target,
            amount,
            applier,
            cardSource
        );
    }

    public static async Task Inflict(
        PlayerChoiceContext choiceContext,
        IEnumerable<Creature> targets,
        InternalDamageInflictVar inflict,
        Creature? applier,
        CardModel? cardSource,
        bool silent = false
    )
    {
        foreach (var target in targets)
            await Inflict(choiceContext, target, inflict, applier, cardSource, silent);
    }

    // Returns the amount of internal damage healed.
    public static async Task<decimal> Heal(
        PlayerChoiceContext choiceContext,
        Creature target,
        InternalDamageHealVar heal,
        Creature? applier,
        CardModel? cardSource,
        bool silent = false
    )
    {
        if (target.GetPower<InternalDamagePower>() is not { } power)
            return 0;

        var amount = IInternalDamageListener.ModifyInternalDamageHealed(target, heal.BaseValue);
        amount = Math.Min(power.Amount, amount);
        if (amount <= 0)
            return 0;

        await PowerCmd.ModifyAmount(choiceContext, power, -amount, applier, cardSource, silent);
        await IInternalDamageListener.InvokeInternalDamageHealed(target, amount);
        return amount;
    }
}
