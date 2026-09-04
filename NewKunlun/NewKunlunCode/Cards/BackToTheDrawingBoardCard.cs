using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Back to the Drawing Board",
    description: "Exhaust your hand.\nNext turn, gain {Energy:energyIcons()} and draw {DrawCards:diff()} cards."
)]
public partial class BackToTheDrawingBoardCard()
    : NewKunlunCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new EnergyVar(2), new DynamicVar(nameof(DrawCards), 2M)];

    protected override void OnUpgrade()
    {
        Energy.UpgradeValueTo(3);
        DrawCards.UpgradeValueTo(3M);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<CardModel> cards = [.. PileType.Hand.GetPile(Owner).Cards];
        foreach (var card in cards)
            await CardCmd.Exhaust(choiceContext, card);

        await PowerCmd.Apply<EnergyNextTurnPower>(
            choiceContext,
            Owner.Creature,
            Energy.BaseValue,
            Owner.Creature,
            this
        );
        await PowerCmd.Apply<DrawCardsNextTurnPower>(
            choiceContext,
            Owner.Creature,
            DrawCards.BaseValue,
            Owner.Creature,
            this
        );
    }
}
