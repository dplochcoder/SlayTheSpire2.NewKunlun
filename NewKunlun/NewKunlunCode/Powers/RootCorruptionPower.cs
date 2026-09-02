using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Cards;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Root Corruption",
    description: "",
    smartDescription: "At the start of your turn, gain {Amount:energyIcons()}, draw {CardDraw} {CardDraw:plural:card|cards}, transform {Amount} {Amount:plural:card|cards} in your hand into [gold]Malfunction[/gold] and discard {Amount:cond:>1?them|it}.",
    selectionScreenPrompt: "Select {Amount} {Amount:plural:card|cards} to transform into [gold]Malfunction[/gold]."
)]
public partial class RootCorruptionPower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar(nameof(CardDraw), 0M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tips.Card<MalfunctionCard>()];

    public override decimal ModifyMaxEnergy(Player player, decimal amount) =>
        amount + (player.Creature == Owner ? Amount : 0);

    public override decimal ModifyHandDraw(Player player, decimal count) =>
        count + (player.Creature == Owner ? CardDraw.BaseValue : 0);

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player
    )
    {
        if (player.Creature != Owner)
            return;

        Flash();
        var cards = await CardSelectCmd.FromHand(
            choiceContext,
            player,
            new CardSelectorPrefs(SelectionScreenPrompt, Amount),
            card => card.IsTransformable && card is not MalfunctionCard,
            this
        );

        List<CardModel> toDiscard = [];
        foreach (var card in cards)
        {
            var result = await CardCmd.TransformTo<MalfunctionCard>(card);
            if (result.HasValue)
                toDiscard.Add(result.Value.cardAdded);
        }
        await CardCmd.Discard(choiceContext, toDiscard);
    }
}
