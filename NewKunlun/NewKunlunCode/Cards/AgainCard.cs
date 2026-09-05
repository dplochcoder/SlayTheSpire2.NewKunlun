using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Again",
    description: "Pull {TalismanDash:cardName()} into your hand. It is free to play this turn."
)]
public partial class AgainCard()
    : NewKunlunCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new TalismanDashVar<AgainCard>(card => TalismanDashCard.IsUpgradedAnywhere(card.Owner))];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override void OnUpgrade() => RemoveKeyword(CardKeyword.Exhaust);

    protected override IEnumerable<IHoverTip> ExtraHoverTips => Tip.TalismanDashCardWithTips(Owner);

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
