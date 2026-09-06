using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.HoverTips;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Tips;

public static class CustomStaticHoverTip
{
    [CustomEnum]
    [StaticHoverTipLocalization(
        title: "Parry Card",
        description: "Any card which grants the [gold]Parry[/gold] effect."
    )]
    public static StaticHoverTip ParryCard;

    [CustomEnum]
    [StaticHoverTipLocalization(
        title: "Precise Parry",
        description: "Full-block an attack with at least one [gold]Parry[/gold].\nThis preserves block."
    )]
    public static StaticHoverTip PreciseParry;
}
