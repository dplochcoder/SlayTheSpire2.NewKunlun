using MegaCrit.Sts2.Core.Entities.Powers;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Dark Steel",
    description: "Increases the power of the [gold]Azure Bow[/gold]."
)]
public class DarkSteelPower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}
