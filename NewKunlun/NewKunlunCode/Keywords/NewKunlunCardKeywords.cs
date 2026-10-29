using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Keywords;

public static class NewKunlunCardKeywords
{
    [CustomEnum]
    [KeywordProperties(AutoKeywordPosition.Before)]
    [KeywordLocalization(
        title: "Quantum",
        description: "Once added to your hand, becomes [gold]Unplayable[/gold] if you play another card."
    )]
    public static CardKeyword Quantum;
}
