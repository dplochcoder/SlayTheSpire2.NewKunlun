using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Modifiers;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Shuanshuan;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Qiankun Board",
    description: "Lose {MaxEnergyLoss:energyIcons()} max energy.\nAt the start of each turn, discard a card from your hand.\nAt the start of your next turn, play this card.",
    flavor: ""
)]
public partial class QiankunBoardRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new EnergyVar(nameof(MaxEnergyLoss), 1)];

    public override decimal ModifyMaxEnergy(Player player, decimal amount) =>
        player == Owner ? Math.Max(amount - MaxEnergyLoss.IntValue, 0) : amount;

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player
    )
    {
        if (player != Owner)
            return;

        List<CardModel> cards =
        [
            .. await CardSelectCmd.FromHandForDiscard(
                choiceContext,
                Owner,
                new(SelectionScreenPrompt, 1),
                c => true,
                this
            ),
        ];
        foreach (var card in cards)
            CardModifier.AddModifier<TacticalModifier>(card);
        await CardCmd.Discard(choiceContext, cards);
    }
}
