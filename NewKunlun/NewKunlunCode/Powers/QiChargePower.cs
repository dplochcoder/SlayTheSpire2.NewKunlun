using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(title: "Qi Charges", description: "", smartDescription: "")]
public class QiChargePower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public static IHoverTip HoverTip() =>
        new HoverTip(
            new LocString("card_keywords", "NEWKUNLUN-QI_CHARGE.title"),
            new LocString("card_keywords", "NEWKUNLUN-QI_CHARGE.description")
        );
}
