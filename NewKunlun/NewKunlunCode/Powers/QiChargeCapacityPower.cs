using MegaCrit.Sts2.Core.Entities.Powers;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Powers;

// Invisible power to track Qi Charge capacity.
[PowerLocalization(title: "Qi Charge Capacity", description: "INTERNAL")]
public class QiChargeCapacityPower : NewKunlunPower
{
    public override PowerType Type => PowerType.None;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override bool IsVisibleInternal => false;
}
