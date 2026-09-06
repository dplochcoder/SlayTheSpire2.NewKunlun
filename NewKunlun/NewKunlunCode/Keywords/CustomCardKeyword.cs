using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Keywords;

public static class CustomCardKeyword
{
    [CustomEnum]
    [KeywordLocalization(
        title: "Boost",
        description: "Deal more damage for each [gold]Discharge[/gold] when you [gold]Detonate[/gold]."
    )]
    public static CardKeyword Boost;

    [CustomEnum]
    [KeywordLocalization(
        title: "Detonate",
        description: "Deal unblockable damage to [gold]Marked[/gold] enemies."
    )]
    public static CardKeyword Detonate;

    [CustomEnum]
    [KeywordLocalization(
        title: "Discharge",
        description: "Spend X [gold]Qi Charges[/gold] or as many as you have, whichever is fewer.\nThe associated effect is conditional on spend."
    )]
    public static CardKeyword Discharge;

    [CustomEnum]
    [KeywordLocalization(
        title: "Mark",
        description: "Mark the enemy with your [gold]Talisman[/gold]\n[gold]Talisman[/gold] disappears after 2 turns, when you [gold]Mark[/gold] again, or when you [gold]Detonate[/gold]."
    )]
    public static CardKeyword Mark;

    [CustomEnum]
    [KeywordLocalization(
        title: "Reload",
        description: "Load an [gold]Azure Sand Magazine[/gold].\nAdd the [gold]Azure Bow[/gold] to your hand if not in your deck."
    )]
    public static CardKeyword Reload;

    [CustomEnum]
    [KeywordLocalization(
        title: "Sharpen",
        description: "Increase the power of the [gold]Azure Bow[/gold].\nThe effect is different on each [gold]Arrow[/gold]."
    )]
    public static CardKeyword Sharpen;
}
