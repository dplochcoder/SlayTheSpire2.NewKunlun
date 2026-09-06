using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Sharpened",
    description: "Increases the power of the [gold]Azure Bow[/gold]."
)]
public class SharpenedPower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.AzureBow()];
}
