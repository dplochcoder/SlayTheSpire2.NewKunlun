using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace NewKunlun.NewKunlunCode.Variables;

public class CustomVar(string name, decimal origValue, Func<Creature?, decimal> fn)
    : DynamicVar(name, origValue)
{
    // `Owner` is marked as non-nullable but this is a lie in the context of HoverTips.
    // ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    public static bool CanCalculate(AbstractModel? owner) =>
        owner
            is CardModel { Owner: not null }
                or PowerModel { Owner: not null }
                or RelicModel { Owner: not null };

    // ReSharper restore ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

    public decimal Calculate(Creature? target = null) =>
        CanCalculate(_owner) ? fn(target) : BaseValue;

    public override void UpdateCardPreview(
        CardModel card,
        CardPreviewMode previewMode,
        Creature? target,
        bool runGlobalHooks
    ) => PreviewValue = runGlobalHooks ? Calculate(target) : BaseValue;

    protected override decimal GetBaseValueForIConvertible() => Calculate();

    public override string ToString() => $"{Calculate()}";
}
