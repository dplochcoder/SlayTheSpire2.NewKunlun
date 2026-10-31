using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Keywords;

public static class ExtraCardKeywords
{
    [CustomEnum]
    [KeywordProperties(AutoKeywordPosition.After)]
    [KeywordLocalization(
        title: "Endemic",
        description: "When this could would be exhausted or transformed, add a copy of it to your discard pile."
    )]
    public static CardKeyword Endemic;

    [CustomEnum]
    [KeywordProperties(AutoKeywordPosition.Before)]
    [KeywordLocalization(
        title: "Quantum",
        description: "Once added to your hand, becomes [gold]Unplayable[/gold] if you play another card."
    )]
    public static CardKeyword Quantum;
}
