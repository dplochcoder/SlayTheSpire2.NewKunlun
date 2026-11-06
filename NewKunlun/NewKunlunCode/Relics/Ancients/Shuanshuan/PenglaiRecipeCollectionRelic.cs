using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Shuanshuan;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Penglai Recipe Collection",
    description: "You can no longer remove cards from your deck.\nLook at {TotalCards} upgraded cards from other characters.\nChoose up to {Cards} cards to add to your deck.",
    flavor: "",
    selectionScreenPrompt: "Choose up to {Cards} cards to add to your deck."
)]
public partial class PenglaiRecipeCollectionRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new(nameof(TotalCards), 30), new(nameof(Cards), 5)];

    public override bool HasUponPickupEffect => true;

    private List<CharacterModel> CharacterPicks()
    {
        List<CharacterModel> others =
        [
            .. ModelDb.AllCharacters.Where(c => c.Id != Owner.Character.Id),
        ];
        Owner.PlayerRng.Rewards.Shuffle(others);
        List<CharacterModel> order = [];
        while (order.Count < TotalCards.IntValue)
            order.AddRange(others.Take(Math.Min(others.Count, TotalCards.IntValue - order.Count)));
        Owner.PlayerRng.Rewards.Shuffle(order);
        return order;
    }

    private IReadOnlyList<CardModel> CardPicks()
    {
        List<CardRarity> rarities = [CardRarity.Common, CardRarity.Uncommon, CardRarity.Rare];
        var characterPicks = CharacterPicks();
        HashSet<ModelId> picked = [];
        List<CardModel> options = [];
        for (var i = 0; i < characterPicks.Count; i++)
        {
            var character = characterPicks[i];
            var rarity = rarities[i % rarities.Count];
            List<CardModel> cards =
            [
                .. character
                    .CardPool.GetUnlockedCards(
                        Owner.UnlockState,
                        Owner.RunState.CardMultiplayerConstraint
                    )
                    .Where(c => c.Rarity == rarity),
            ];
            if (cards.Any(c => !picked.Contains(c.Id)))
                cards = [.. cards.Where(c => !picked.Contains(c.Id))];
            var card = Owner.PlayerRng.Rewards.NextItem(cards)!.CreateCloneForPlayer(Owner);
            if (card.IsUpgradable)
                CardCmd.Upgrade(card);
            picked.Add(card.Id);
            options.Add(card);
        }

        return [.. options.OrderBy(c => c.Rarity)];
    }

    public override async Task AfterObtained()
    {
        var picks = await CardSelectCmd.FromSimpleGrid(
            new BlockingPlayerChoiceContext(),
            CardPicks(),
            Owner,
            new(SelectionScreenPrompt, 0, Cards.IntValue)
            {
                Cancelable = false,
                RequireManualConfirmation = true,
            }
        );
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(picks, PileType.Deck.GetPile(Owner)));
    }

    [HarmonyPatch]
    private static class Patches
    {
        [HarmonyPatch(typeof(CardModel), nameof(CardModel.IsRemovable), MethodType.Getter)]
        [HarmonyPostfix]
        private static void IsRemovable(
            // ReSharper disable once InconsistentNaming
            CardModel __instance,
            // ReSharper disable once InconsistentNaming
            ref bool __result
        )
        {
            if (__instance.Owner.Relics.Any(r => r is PenglaiRecipeCollectionRelic))
                __result = false;
        }
    }
}
