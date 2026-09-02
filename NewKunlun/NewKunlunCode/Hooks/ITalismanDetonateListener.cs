using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;

namespace NewKunlun.NewKunlunCode.Hooks;

public interface ITalismanDetonateListener
{
    decimal AdditiveModifier(decimal amount, Creature? dealer) => 0;

    decimal MultiplicativeModifier(decimal amount, Creature? dealer) => 1;

    Task OnTalismanDetonated(PlayerChoiceContext choiceContext, decimal amount, Creature? dealer) =>
        Task.CompletedTask;

    public static decimal ModifyTalismanDetonateDamage(
        ICombatState combatState,
        decimal amount,
        Creature? applier
    )
    {
        decimal add = 0;
        decimal multiply = 1;
        foreach (
            var model in Hook.IterateCombatHookListeners(combatState)
                .OfType<ITalismanDetonateListener>()
        )
        {
            add += model.AdditiveModifier(amount, applier);
            multiply *= model.MultiplicativeModifier(amount, applier);
        }

        return Math.Max(0, amount + add) * multiply;
    }

    public static async Task InvokeTalismanDetonated(
        ICombatState combatState,
        PlayerChoiceContext choiceContext,
        decimal amount,
        Creature dealer
    )
    {
        List<ITalismanDetonateListener> listeners =
        [
            .. Hook.IterateCombatHookListeners(combatState!).OfType<ITalismanDetonateListener>(),
        ];
        foreach (var listener in listeners)
            await listener.OnTalismanDetonated(choiceContext, amount, dealer);
    }
}
