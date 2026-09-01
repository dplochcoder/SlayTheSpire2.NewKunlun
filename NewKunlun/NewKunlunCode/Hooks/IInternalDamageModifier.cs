using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace NewKunlun.NewKunlunCode.Hooks;

public interface IInternalDamageModifier
{
    decimal AdditiveModifier(
        Creature? target,
        decimal amount,
        Creature? applier,
        CardModel? source
    ) => 0;

    decimal MultiplicativeModifier(
        Creature? target,
        decimal amount,
        Creature? applier,
        CardModel? source
    ) => 1;

    public static decimal ModifyInternalDamage(
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
                .OfType<IInternalDamageModifier>()
        )
        {
            add += model.AdditiveModifier(target, amount, applier, cardSource);
            multiply *= model.MultiplicativeModifier(target, amount, applier, cardSource);
        }

        return Math.Max(0, amount + add) * multiply;
    }
}
