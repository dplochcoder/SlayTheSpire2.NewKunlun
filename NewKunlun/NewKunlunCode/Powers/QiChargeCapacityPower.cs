using MegaCrit.Sts2.Core.Entities.Powers;

namespace NewKunlun.NewKunlunCode.Powers;

// Invisible power to track Qi Charge capacity.
[Localization.PowerLocalization("Qi Charge Capacity", "INTERNAL", "INTERNAL")]
public class QiChargeCapacityPower : NewKunlunPower
{
    public override PowerType Type => PowerType.None;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override bool IsVisibleInternal => false;
}
