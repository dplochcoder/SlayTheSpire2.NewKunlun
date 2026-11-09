using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards.Ancients;

[Pool(typeof(SilentCardPool))]
[CardLocalization(
    title: "Fortify",
    description: "Gain {Block:diff()} [gold]Block[/gold].\nDiscard all [gold]Sly[/gold] cards in your hand.\nDiscard 1 card.\n"
)]
public partial class FortifyCard()
    : NewKunlunCard(1, CardType.Skill, CardRarity.Ancient, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(16, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.Static(StaticHoverTip.Block)];

    protected override void OnUpgrade() => Block.UpgradeValueTo(22);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, Block, cardPlay);

        List<CardModel> toDiscard =
        [
            .. PileType.Hand.GetPile(Owner).Cards.Where(c => c.IsSlyThisTurn),
        ];
        await CardCmd.Discard(choiceContext, toDiscard);

        toDiscard =
        [
            .. await CardSelectCmd.FromHandForDiscard(
                choiceContext,
                Owner,
                new(CardSelectorPrefs.DiscardSelectionPrompt, 1),
                _ => true,
                this
            ),
        ];
        await CardCmd.Discard(choiceContext, toDiscard);
    }
}
