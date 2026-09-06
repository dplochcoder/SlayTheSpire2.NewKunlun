using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Keywords;

public static class CustomCardKeyword
{
    [CustomEnum]
    [KeywordProperties(AutoKeywordPosition.Before)]
    [KeywordLocalization(
        title: "Ephemeral",
        description: "Once drawn, [gold]Exhausts[/gold] after it's played, discarded, or at the end of the turn."
    )]
    public static CardKeyword Ephemeral;
}
