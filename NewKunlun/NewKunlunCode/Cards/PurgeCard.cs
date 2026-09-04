using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Purge",
    description: "Choose one [gold]Status[/gold] or [gold]Curse[/gold] in your hand and [gold]Exhaust[/gold] it to gain {Energy:energyIcons()}. Draw 2 cards.",
    selectionScreenPrompt: "Choose a card to Exhaust."
)]
public partial class PurgeCard()
    : NewKunlunCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);

    protected override bool ShouldGlowGoldInternal =>
        PileType.Hand.GetPile(Owner).Cards.Any(c => c.Type is CardType.Status or CardType.Curse);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var exhausts = await CardSelectCmd.FromHand(
            choiceContext,
            Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 0, 1),
            card => card.Type is CardType.Status or CardType.Curse,
            this
        );
        List<CardModel> list = [.. exhausts];
        if (list.Count > 0)
        {
            foreach (var card in list)
                await CardCmd.Exhaust(choiceContext, card);
            await PlayerCmd.GainEnergy(Energy.BaseValue, Owner);
        }

        await CardPileCmd.Draw(choiceContext, 2, Owner);
    }
}
