using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace NewKunlun.NewKunlunCode.Variables;

public class QiChargeVar(string name, decimal charges) : DynamicVar(name, charges)
{
    public const string DefaultName = "QiCharge";

    public QiChargeVar(decimal charges)
        : this(DefaultName, charges) { }

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
