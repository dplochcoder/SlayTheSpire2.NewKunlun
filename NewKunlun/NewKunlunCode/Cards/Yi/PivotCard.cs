using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards.Yi;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Feint",
    description: "Draw a card.\nChoose {CardCount:plural:a card|2 cards} in your hand. {CardCount:plural:It gains|They gain} [gold]Retain[/gold] this turn.",
    selectionScreenPrompt: "Choose a card to place on top of your draw pile."
)]
public partial class PivotCard()
    : NewKunlunCard(0, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new(nameof(CardCount), 1)];

    protected override void OnUpgrade() => CardCount.UpgradeValueTo(2M);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, Owner);

        var cards = await CardSelectCmd.FromHand(
            choiceContext,
            Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, CardCount.IntValue),
            _ => true,
            this
        );
        foreach (var card in cards)
            card.GiveSingleTurnRetain();
    }
}
