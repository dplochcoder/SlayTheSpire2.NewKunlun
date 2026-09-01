using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Hooks;

namespace NewKunlun.NewKunlunCode.Variables;

public class TalismanDetonateDamageVar(decimal damage)
    : DamageVar("TalismanDetonateDamage", damage, ValueProp.Unblockable | ValueProp.Unpowered)
{
    public override void UpdateCardPreview(
        CardModel card,
        CardPreviewMode previewMode,
        Creature? target,
        bool runGlobalHooks
    ) =>
        PreviewValue = ITalismanDetonateDamageModifier.ModifyTalismanDetonateDamage(
            card.CombatState!,
            BaseValue,
            card.Owner.Creature
        );
}
