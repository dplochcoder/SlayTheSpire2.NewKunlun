using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace NewKunlun.NewKunlunCode.Variables;

public class CustomDamageVar<T>(
    string name,
    decimal origValue,
    ValueProp props,
    Func<T, Creature?, decimal> fn
) : CustomVar<T>(name, origValue, fn)
    where T : AbstractModel
{
    public override void UpdateCardPreview(
        CardModel card,
        CardPreviewMode previewMode,
        Creature? target,
        bool runGlobalHooks
    )
    {
        var baseValue = Calculate(target);
        var damage = baseValue;
        if (card.Enchantment is { } enchantment)
        {
            damage += enchantment.EnchantDamageAdditive(damage, props);
            damage *= enchantment.EnchantDamageMultiplicative(damage, props);
            if (!card.IsEnchantmentPreview)
                EnchantedValue = damage;
        }
        if (runGlobalHooks)
            damage = Hook.ModifyDamage(
                card.Owner.RunState,
                card.CombatState,
                target,
                card.Owner.Creature,
                baseValue,
                props,
                card,
                null,
                ModifyDamageHookType.All,
                previewMode,
                out var _
            );
        PreviewValue = damage;
    }
}
