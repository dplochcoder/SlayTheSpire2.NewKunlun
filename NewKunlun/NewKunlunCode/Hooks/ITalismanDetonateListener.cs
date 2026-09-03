using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;

namespace NewKunlun.NewKunlunCode.Hooks;

public interface ITalismanDetonateListener
{
    decimal BaseDamageAdditiveModifier(decimal amount, Creature? dealer) => 0;

    decimal BaseDamageMultiplicativeModifier(decimal amount, Creature? dealer) => 1;

    Task OnTalismanDetonated(
        PlayerChoiceContext choiceContext,
        int qiCharges,
        decimal totalDamage,
        Creature? dealer
    ) => Task.CompletedTask;

    public static decimal ModifyTalismanDetonateBaseDamage(
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
            add += model.BaseDamageAdditiveModifier(amount, applier);
            multiply *= model.BaseDamageMultiplicativeModifier(amount, applier);
        }

        return Math.Max(0, amount + add) * multiply;
    }

    public static async Task InvokeTalismanDetonated(
        ICombatState combatState,
        PlayerChoiceContext choiceContext,
        int qiCharges,
        decimal totalDamage,
        Creature dealer
    )
    {
        List<ITalismanDetonateListener> listeners =
        [
            .. Hook.IterateCombatHookListeners(combatState!).OfType<ITalismanDetonateListener>(),
        ];
        foreach (var listener in listeners)
            await listener.OnTalismanDetonated(choiceContext, qiCharges, totalDamage, dealer);
    }
}
