using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace NewKunlun.NewKunlunCode.Variables;

public abstract class CustomVar<T>(string name) : DynamicVar(name, 0M)
    where T : AbstractModel
{
    private new T? _owner;

    public override void SetOwner(AbstractModel owner)
    {
        base.SetOwner(owner);
        _owner = (T)owner;
        UpdateValue();
    }

    public void UpdateValue() => BaseValue = Calculate();

    protected abstract decimal Calculate(T owner, Creature? target);

    private decimal Calculate(Creature? target = null) =>
        _owner != null ? Calculate(_owner, target) : 0;

    public override void UpdateCardPreview(
        CardModel card,
        CardPreviewMode previewMode,
        Creature? target,
        bool runGlobalHooks
    ) => PreviewValue = Calculate(target);

    protected override decimal GetBaseValueForIConvertible() => Calculate();

    public override string ToString() => $"{Calculate()}";
}
