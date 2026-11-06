using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Runs;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Lear;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Celestial Passkey",
    description: "Remove 2 random cards from your deck.\nAdd 1 card of your choice to your deck.",
    flavor: "",
    selectionScreenPrompt: "Choose a card to add to your deck."
)]
public class CelestialPasskeyRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool IsAllowedForPlayer(Player player) =>
        player.Deck.Cards.Count(c => c.IsRemovable) >= 2;

    public override bool HasUponPickupEffect => true;

    private List<CardCreationResult> CreateCards()
    {
        List<CardCreationResult> results =
        [
            .. Owner
                .Character.CardPool.CardsUnlockedFor(Owner)
                .Select(canonical => new CardCreationResult(
                    Owner.RunState.CreateCard(canonical, Owner)
                )),
        ];

        CardCreationOptions options = new(
            [Owner.Character.CardPool],
            CardCreationSource.Other,
            CardRarityOddsType.Uniform
        );
        options = options.WithFlags(
            CardCreationFlags.IsCardReward | CardCreationFlags.NoUpgradeRoll
        );
        if (
            Hook.TryModifyCardRewardOptions(
                Owner.RunState,
                Owner,
                results,
                options,
                out var modifiers
            )
        )
            TaskHelper.RunSafely(Hook.AfterModifyingCardRewardOptions(Owner.RunState, modifiers));

        return results;
    }

    public override async Task AfterObtained()
    {
        var cards = Owner.PlayerRng.Rewards.NextItems(
            2,
            Owner.Deck.Cards.Where(c => c.IsRemovable)
        );
        await CardPileCmd.RemoveFromDeck([.. cards]);

        var newCard = CardSelectCmd.FromSimpleGridForRewards(
            new BlockingPlayerChoiceContext(),
            CreateCards(),
            Owner,
            new(SelectionScreenPrompt, 1)
        );
    }
}
