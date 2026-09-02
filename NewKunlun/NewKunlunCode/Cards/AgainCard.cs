using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Again",
    description: "Pull [gold]Talisman Dash[/gold] into your hand. It is free to play this turn."
)]
public class AgainCard() : NewKunlunCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        IsUpgraded ? [] : [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        Tips.TalismanDashCardWithTips(Owner);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var card = Owner.FindCard<TalismanDashCard>([
            PileType.Deck,
            PileType.Discard,
            PileType.Hand,
        ]);
        if (card == null)
            return;

        await CardPileCmd.Add(card, PileType.Hand.GetPile(Owner));
        card.SetToFreeThisTurn();
    }
}
