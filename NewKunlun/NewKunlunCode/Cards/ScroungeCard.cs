using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Scrounge",
    description: "Discard your hand. Draw {DrawCards} cards. Keep {KeepCards:diff()} and discard the rest.",
    selectionScreenPrompt: "Select up to {KeepCards} cards to keep."
)]
public partial class ScroungeCard()
    : NewKunlunCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar(nameof(DrawCards), 8M), new DynamicVar(nameof(KeepCards), 2M)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardCmd.Discard(choiceContext, PileType.Hand.GetPile(Owner).Cards);
        await CardPileCmd.Draw(choiceContext, DrawCards.BaseValue, Owner);
        var keep = await CardSelectCmd.FromHand(
            choiceContext,
            cardPlay.Player,
            new CardSelectorPrefs(SelectionScreenPrompt, (int)KeepCards.BaseValue),
            _ => true,
            this
        );
        await CardCmd.Discard(
            choiceContext,
            PileType.Hand.GetPile(Owner).Cards.Where(c => !keep.Contains(c))
        );
    }
}
