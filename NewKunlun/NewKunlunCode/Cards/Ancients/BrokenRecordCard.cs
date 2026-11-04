using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards.Ancients;

[Pool(typeof(EventCardPool))]
[CardLocalization(title: "Broken Record", description: "How could you be so careless!")]
public class BrokenRecordCard()
    : NewKunlunCard(0, CardType.Curse, CardRarity.Ancient, TargetType.None)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Eternal, CardKeyword.Unplayable];

    public override int MaxUpgradeLevel => 0;
}
