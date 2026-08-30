using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace NewKunlun.NewKunlunCode.Variables;

public class InternalDamageVar(string name, Decimal damage) : DynamicVar(name, damage)
{
    public const string DefaultName = "InternalDamage";

    public InternalDamageVar(Decimal damage)
        : this(DefaultName, damage) { }

    public override void UpdateCardPreview(
        CardModel card,
        CardPreviewMode previewMode,
        Creature? target,
        bool runGlobalHooks
    )
    {
        // TODO: Hook modifiers.
        var value = BaseValue;
        PreviewValue = value;
    }
}
