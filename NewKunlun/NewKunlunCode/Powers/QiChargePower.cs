using MegaCrit.Sts2.Core.Entities.Powers;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Qi Charges",
    description: "Energy used to accelerate damage. Qi Charges are spent automatically by cards that request them.",
    smartDescription: ""
)]
public class QiChargePower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}
