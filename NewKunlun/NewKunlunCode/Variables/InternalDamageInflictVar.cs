using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Hooks;

namespace NewKunlun.NewKunlunCode.Variables;

public class InternalDamageInflictVar(string name, decimal damage) : DynamicVar(name, damage)
{
    public InternalDamageInflictVar(decimal damage)
        : this("InternalDamageInflict", damage) { }

    protected virtual Creature? ModifyTarget(CardModel card, Creature? origTarget) => origTarget;

    public override void UpdateCardPreview(
        CardModel card,
        CardPreviewMode previewMode,
        Creature? target,
        bool runGlobalHooks
    )
    {
        if (runGlobalHooks && card.CombatState != null)
            PreviewValue = IInternalDamageListener.ModifyInternalDamageInflicted(
                card.CombatState,
                ModifyTarget(card, target),
                BaseValue,
                card.Owner.Creature,
                card
            );
        else
            PreviewValue = BaseValue;
    }
}
