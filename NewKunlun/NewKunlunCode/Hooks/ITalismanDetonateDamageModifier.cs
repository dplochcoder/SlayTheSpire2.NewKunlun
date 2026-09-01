using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;

namespace NewKunlun.NewKunlunCode.Hooks;

public interface ITalismanDetonateDamageModifier
{
    decimal AdditiveModifier(decimal amount, Creature? applier) => 0;

    decimal MultiplicativeModifier(decimal amount, Creature? applier) => 1;

    Task TalismanDetonated() => Task.CompletedTask;

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
                .OfType<ITalismanDetonateDamageModifier>()
        )
        {
            add += model.AdditiveModifier(amount, applier);
            multiply *= model.MultiplicativeModifier(amount, applier);
        }

        return Math.Max(0, amount + add) * multiply;
    }
}
