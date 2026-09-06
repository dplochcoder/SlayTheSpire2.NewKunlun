using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Hooks;

namespace NewKunlun.NewKunlunCode.Variables;

public class TalismanDetonateBaseDamageVar(decimal damage)
    : DamageVar("TalismanDetonateBaseDamage", damage, ValueProp.Unblockable | ValueProp.Unpowered)
{
    public decimal Calculate(CardModel card) =>
        card.CombatState != null
            ? ITalismanDetonateListener.ModifyTalismanDetonateBaseDamage(
                card.CombatState,
                BaseValue,
                card.Owner.Creature
            )
            : BaseValue;

    public override void UpdateCardPreview(
        CardModel card,
        CardPreviewMode previewMode,
        Creature? target,
        bool runGlobalHooks
    ) => PreviewValue = runGlobalHooks ? Calculate(card) : BaseValue;
}
