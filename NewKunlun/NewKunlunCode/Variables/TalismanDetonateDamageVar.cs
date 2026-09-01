using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Powers;

namespace NewKunlun.NewKunlunCode.Variables;

public class TalismanDetonateDamageVar(string name, decimal value)
    : DamageVar(name, value, ValueProp.Unblockable | ValueProp.Unpowered)
{
    public TalismanDetonateDamageVar(decimal value)
        : this("TalismanDetonateDamage", value) { }

    public override void UpdateCardPreview(
        CardModel card,
        CardPreviewMode previewMode,
        Creature? target,
        bool runGlobalHooks
    )
    {
        base.UpdateCardPreview(card, previewMode, target, runGlobalHooks);

        PreviewValue += card.Owner.Creature.GetPowerAmount<FullControlPower>();
    }
}
