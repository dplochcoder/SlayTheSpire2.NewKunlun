using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Kuafu;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Compactor",
    description: "At the start of your turn, look at the top {LookCards} of your deck.\nDiscard {DiscardCards} of them.",
    flavor: ""
)]
public partial class CompactorRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new(nameof(LookCards), 4), new(nameof(DiscardCards), 2)];

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player
    )
    {
        if (player != Owner || Owner.PlayerCombatState is not { } pcs)
            return;

        if (pcs.DrawPile.Cards.Count < LookCards.IntValue && pcs.DiscardPile.Cards.Count > 0)
            await CardPileCmd.Shuffle(choiceContext, Owner);

        List<CardModel> toDiscard =
        [
            .. await CardSelectCmd.FromSimpleGrid(
                choiceContext,
                [.. pcs.DrawPile.Cards.Take(LookCards.IntValue)],
                Owner,
                new(CardSelectorPrefs.DiscardSelectionPrompt, 2)
            ),
        ];
        await CardCmd.Discard(choiceContext, toDiscard);
    }
}
