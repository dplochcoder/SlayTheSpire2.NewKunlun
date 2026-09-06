using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Qi Charges",
    description: "Energy used to accelerate damage.\nConsumed by cards with the [gold]Discharge[/gold] keyword.",
    smartDescription: ""
)]
public class QiChargePower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.Discharge()];
}
