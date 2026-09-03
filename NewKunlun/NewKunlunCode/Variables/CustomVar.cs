using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace NewKunlun.NewKunlunCode.Variables;

public class CustomVar<T>(string name, decimal origValue, Func<T, Creature?, decimal> fn)
    : DynamicVar(name, origValue)
    where T : AbstractModel
{
    private new T? _owner;

    public override void SetOwner(AbstractModel owner)
    {
        base.SetOwner(owner);
        _owner = (T)owner;
    }

    public decimal Calculate(Creature? target = null) =>
        _owner != null ? fn(_owner, target) : BaseValue;

    public override void UpdateCardPreview(
        CardModel card,
        CardPreviewMode previewMode,
        Creature? target,
        bool runGlobalHooks
    ) => PreviewValue = Calculate(target);

    protected override decimal GetBaseValueForIConvertible() => Calculate();

    public override string ToString() => $"{Calculate()}";
}
