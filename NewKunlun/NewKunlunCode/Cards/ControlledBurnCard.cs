using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Controlled Burn",
    description: "Transform 2 cards in your hand into [gold]Smolder[/gold]. Gain {Energy:energyIcons()}. Add 1 [gold]Smolder[/gold] to your discard pile."
)]
public partial class ControlledBurnCard()
    : NewKunlunCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.Smolder()];

    protected override void OnUpgrade() => Energy.UpgradeValueTo(2);

    private bool IsEligibleCard(CardModel c) =>
        c != this && c is not SmolderCard && c.IsTransformable;

    private IEnumerable<CardModel> EligibleCards() =>
        PileType.Hand.GetPile(Owner).Cards.Where(IsEligibleCard);

    protected override bool IsPlayable => EligibleCards().Count() >= 2;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<CardModel> cards = [.. EligibleCards()];
        if (cards.Count < 2)
            return;

        cards =
        [
            .. await CardSelectCmd.FromHand(
                choiceContext,
                Owner,
                new CardSelectorPrefs(SelectionScreenPrompt, 2),
                IsEligibleCard,
                this
            ),
        ];
        foreach (var card in cards)
            await CardCmd.TransformTo<SmolderCard>(card);

        await PlayerCmd.GainEnergy(Energy.BaseValue, Owner);
        await this.AddGeneratedCardToPile<SmolderCard>(PileType.Discard);
    }
}
