using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace NewKunlun.NewKunlunCode.Modifiers;

public abstract class NewKunlunModifier : CardModifier
{
    public static IEnumerable<IHoverTip> HoverTips<T>()
        where T : NewKunlunModifier
    {
        var modifier = ModelDb.CardModifier<T>();
        List<IHoverTip> tips =
        [
            new HoverTip(modifier.GetLoc("title"), modifier.GetLoc("description")),
            .. modifier.ExtraHoverTips,
        ];
        return tips;
    }

    public enum ModifierStackType
    {
        Single,
        Instanced,
        Counter,
    }

    public override void OnInitialApplication() => OnAmountChanged(Amount);

    public abstract ModifierStackType StackType { get; }

    public virtual bool CanModify(CardModel cardModel) => true;

    public virtual IEnumerable<IHoverTip> ExtraHoverTips => [];

    public override void AddTips(List<IHoverTip> tips) => tips.AddRange(ExtraHoverTips);

    public override bool ApplyStacked(CardModifier newApplied)
    {
        switch (StackType)
        {
            case ModifierStackType.Single:
                return true;
            case ModifierStackType.Instanced:
                return false;
            case ModifierStackType.Counter:
                Amount += newApplied.Amount;
                OnAmountChanged(newApplied.Amount);
                return true;
            default:
                return false;
        }
    }

    protected virtual void OnAmountChanged(int change) { }
}
