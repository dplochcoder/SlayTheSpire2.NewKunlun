using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Controlled Burn",
    description: "Transform 2 cards in your hand into [gold]Smolder[/gold].\nGain {Energy:energyIcons()}.\nAdd 1 [gold]Smolder[/gold] to your discard pile.",
    selectionScreenPrompt: "Choose 2 cards to transform into [gold]Smolder[/gold]"
)]
public partial class ControlledBurnCard()
    : NewKunlunCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<SmolderCard>()];

    protected override void OnUpgrade() => Energy.UpgradeValueTo(3);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cards = await CardSelectCmd.FromHand(
            choiceContext,
            Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 2),
            c => c is not SmolderCard && c.IsTransformable,
            this
        );
        foreach (var card in cards)
            await CardCmd.TransformTo<SmolderCard>(card);

        await PlayerCmd.GainEnergy(Energy.BaseValue, Owner);
        await Owner.AddGeneratedCardToPile<SmolderCard>(PileType.Discard);
    }
}
