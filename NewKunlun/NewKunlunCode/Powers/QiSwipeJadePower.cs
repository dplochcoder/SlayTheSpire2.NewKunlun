using MegaCrit.Sts2.Core.Entities.Powers;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Qi Swipe Jade",
    description: "Whenever you successfully [gold]Parry[/gold], gain {Amount:plural:an extra [gold]Qi Charge[/gold]|{Amount} extra [gold]Qi Charges[/gold]}."
)]
public class QiSwipeJadePower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}
