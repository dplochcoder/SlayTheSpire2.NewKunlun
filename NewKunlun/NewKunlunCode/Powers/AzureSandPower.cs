using MegaCrit.Sts2.Core.Entities.Powers;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Azure Sand",
    description: "Resource spent to fire the [gold]Azure Bow[/gold]. Spawns the bow into your hand if you do not have it yet."
)]
public class AzureSandPower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}
