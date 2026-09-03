using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace NewKunlun.NewKunlunCode.Hooks;

public interface IInternalDamageListener
{
    decimal DamageAdditiveModifier(
        Creature? target,
        decimal amount,
        Creature? applier,
        CardModel? source
    ) => 0;

    decimal DamageMultiplicativeModifier(
        Creature? target,
        decimal amount,
        Creature? applier,
        CardModel? source
    ) => 1;

    Task OnInternalDamageTaken(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount,
        Creature? applier,
        CardModel? source
    ) => Task.CompletedTask;

    Task OnInternalDamageResolved(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount
    ) => Task.CompletedTask;

    decimal HealingAdditiveModifier(Creature? target, decimal amount) => 0;

    Task OnInternalDamageHealed(Creature target, decimal amount) => Task.CompletedTask;

    public static decimal ModifyInternalDamageInflicted(
        ICombatState combatState,
        Creature? target,
        decimal amount,
        Creature? applier,
        CardModel? cardSource
    )
    {
        decimal add = 0;
        decimal multiply = 1;
        foreach (
            var model in Hook.IterateCombatHookListeners(combatState)
                .OfType<IInternalDamageListener>()
        )
        {
            add += model.DamageAdditiveModifier(target, amount, applier, cardSource);
            multiply *= model.DamageMultiplicativeModifier(target, amount, applier, cardSource);
        }

        return Math.Max(0, amount + add) * multiply;
    }

    public static async Task InvokeInternalDamageTaken(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount,
        Creature? applier,
        CardModel? cardSource
    )
    {
        List<IInternalDamageListener> listeners =
        [
            .. Hook.IterateCombatHookListeners(target.CombatState!)
                .OfType<IInternalDamageListener>(),
        ];
        foreach (var listener in listeners)
            await listener.OnInternalDamageTaken(
                choiceContext,
                target,
                amount,
                applier,
                cardSource
            );
    }

    public static async Task InvokeInternalDamageResolved(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount
    )
    {
        List<IInternalDamageListener> listeners =
        [
            .. Hook.IterateCombatHookListeners(target.CombatState!)
                .OfType<IInternalDamageListener>(),
        ];
        foreach (var listener in listeners)
            await listener.OnInternalDamageResolved(choiceContext, target, amount);
    }

    public static decimal ModifyInternalDamageHealed(Creature target, decimal amount)
    {
        decimal add = 0;
        foreach (
            var model in Hook.IterateCombatHookListeners(target.CombatState!)
                .OfType<IInternalDamageListener>()
        )
            add += model.HealingAdditiveModifier(target, amount);

        return Math.Max(0, amount + add);
    }

    public static async Task InvokeInternalDamageHealed(Creature target, decimal amount)
    {
        List<IInternalDamageListener> listeners =
        [
            .. Hook.IterateCombatHookListeners(target.CombatState!)
                .OfType<IInternalDamageListener>(),
        ];
        foreach (var listener in listeners)
            await listener.OnInternalDamageHealed(target, amount);
    }
}
