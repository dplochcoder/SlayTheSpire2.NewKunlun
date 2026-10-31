using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards.Statuses;

[Pool(typeof(StatusCardPool))]
[CardLocalization(
    title: "Trip",
    description: "While this card is in your hand, you cannot gain [gold]Block[/gold]."
)]
public class TripCard() : NewKunlunCard(1, CardType.Status, CardRarity.Status, TargetType.None)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Ethereal, CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.Static(StaticHoverTip.Block)];

    protected override bool ShouldGlowGoldInternal =>
        Owner.PlayerCombatState?.Hand.Cards.Any(c => c.GainsBlock) ?? false;

    public override decimal ModifyBlockAdditive(
        Creature target,
        decimal block,
        ValueProp props,
        CardModel? cardSource,
        CardPlay? cardPlay
    ) => Pile?.Type is PileType.Hand && target == Owner.Creature ? 0 : block;
}
