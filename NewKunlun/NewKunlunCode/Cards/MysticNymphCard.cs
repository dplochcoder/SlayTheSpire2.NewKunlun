using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(TokenCardPool))]
[CardLocalization(
    title: "Mystic Nymph",
    description: "Draw {DrawCards:diff()} cards.\nPlace {PlaceCards:diff()} cards on top of your deck."
)]
public partial class MysticNymphCard()
    : NewKunlunCard(0, CardType.Skill, CardRarity.Ancient, TargetType.None)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new(nameof(DrawCards), 5), new(nameof(PlaceCards), 5)];

    protected override void OnUpgrade() => DrawCards.UpgradeValueTo(7);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, DrawCards.BaseValue, Owner);
        List<CardModel> cards =
        [
            .. await CardSelectCmd.FromHand(
                choiceContext,
                Owner,
                new CardSelectorPrefs(
                    SelectionScreenPrompt,
                    PlaceCards.IntValue,
                    PlaceCards.IntValue
                ),
                _ => true,
                this
            ),
        ];
        cards = [.. cards.OrderBy(c => c.NetId)];

        Owner.RunState.Rng.CombatCardSelection.Shuffle(cards);
        foreach (var card in cards)
            await CardPileCmd.Add(card, PileType.Deck.GetPile(Owner), CardPilePosition.Top);
    }
}
