using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using NewKunlun.NewKunlunCode.Cards;
using NewKunlun.NewKunlunCode.Cards.Yi;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Sharpened",
    description: "Increases the power of the [gold]Azure Bow[/gold]."
)]
public class SharpenedPower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<AzureBowCard>()];
}
