using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Forecast",
    description: "Look at 10 random cards from your deck, in order. Discard up to 5 of them and preserve the rest."
)]
public class ForecastCard() : NewKunlunCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Ethereal, CardKeyword.Exhaust];

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Innate);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<CardModel> drawPile = [.. PileType.Draw.GetPile(Owner).Cards];
        List<(CardModel card, int index)> cards = [.. drawPile.Select((t, i) => (t, i))];
        cards.StableShuffle(CombatState!.RunState.Rng.CombatCardSelection);

        var toDiscard = await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            [.. cards.Take(10).OrderBy(p => p.index).Select(p => p.card)],
            Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 0, 5)
        );
        await CardCmd.Discard(choiceContext, toDiscard);
    }
}
