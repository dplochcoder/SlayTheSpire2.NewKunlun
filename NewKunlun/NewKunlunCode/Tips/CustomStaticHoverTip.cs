using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.HoverTips;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Tips;

public static class CustomStaticHoverTip
{
    [CustomEnum]
    [StaticHoverTipLocalization(
        title: "Boost",
        description: "Deal more damage for each [gold]Discharge[/gold] when you [gold]Detonate[/gold]."
    )]
    public static StaticHoverTip Boost;

    [CustomEnum]
    [StaticHoverTipLocalization(
        title: "Detonate",
        description: "Deal unblockable damage to [gold]Marked[/gold] enemies."
    )]
    public static StaticHoverTip Detonate;

    [CustomEnum]
    [StaticHoverTipLocalization(
        title: "Discharge",
        description: "Spend X [gold]Qi Charges[/gold] or as many as you have, whichever is fewer.\nThe associated effect is conditional on spend."
    )]
    public static StaticHoverTip Discharge;

    [CustomEnum]
    [StaticHoverTipLocalization(
        title: "Mark",
        description: "Mark the enemy with your [gold]Talisman[/gold]\n[gold]Talisman[/gold] disappears after 2 turns, when you [gold]Mark[/gold] again, or when you [gold]Detonate[/gold]."
    )]
    public static StaticHoverTip Mark;

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

    [CustomEnum]
    [StaticHoverTipLocalization(
        title: "Reload",
        description: "Load an [gold]Azure Sand Magazine[/gold].\nAdd the [gold]Azure Bow[/gold] to your hand if not in your deck."
    )]
    public static StaticHoverTip Reload;

    [CustomEnum]
    [StaticHoverTipLocalization(
        title: "Sharpen",
        description: "Increase the power of the [gold]Azure Bow[/gold].\nThe effect is different on each [gold]Arrow[/gold]."
    )]
    public static StaticHoverTip Sharpen;
}
