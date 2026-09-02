using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Hooks;

namespace NewKunlun.NewKunlunCode.Variables;

public class InternalDamageInflictVar(string name, decimal damage) : DynamicVar(name, damage)
{
    public const string DefaultName = "InternalDamageInflict";

    public InternalDamageInflictVar(decimal damage)
        : this(DefaultName, damage) { }

    public override void UpdateCardPreview(
        CardModel card,
        CardPreviewMode previewMode,
        Creature? target,
        bool runGlobalHooks
    ) =>
        PreviewValue = IInternalDamageListener.ModifyInternalDamageInflicted(
            card.CombatState!,
            target,
            BaseValue,
            card.Owner.Creature,
            card
        );
}
