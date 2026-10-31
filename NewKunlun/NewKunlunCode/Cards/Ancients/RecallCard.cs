using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards.Ancients;

[Pool(typeof(EventCardPool))]
[CardLocalization(
    title: "Recall",
    description: "Shuffle all cards in your hand and discard pile into your draw pile.\nChoose {Cards:diff()} cards from your deck to put in your hand."
)]
public partial class RecallCard()
    : NewKunlunCard(0, CardType.Skill, CardRarity.Ancient, TargetType.None)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Exhaust, CardKeyword.Innate];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new(nameof(Cards), 3)];

    protected override void OnUpgrade() => Cards.UpgradeValueTo(4);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<CardModel> toShuffle =
        [
            .. PileType.Draw.GetPile(Owner).Cards.Concat(PileType.Discard.GetPile(Owner).Cards),
        ];
        foreach (var card in toShuffle)
            await CardPileCmd.Add(card, PileType.Draw);
        await CardPileCmd.Shuffle(choiceContext, Owner);

        var toDraw = await CardSelectCmd.FromCombatPile(
            choiceContext,
            PileType.Draw.GetPile(Owner),
            Owner,
            new(SelectionScreenPrompt, Cards.IntValue, Cards.IntValue)
        );
        foreach (var card in toDraw)
            await CardPileCmd.Add(card, PileType.Hand);
    }
}
