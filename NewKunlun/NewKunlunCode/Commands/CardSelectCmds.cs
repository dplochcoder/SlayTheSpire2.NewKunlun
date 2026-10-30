using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Runs.History;

namespace NewKunlun.NewKunlunCode.Commands;

public static class CardSelectCmds
{
    public static async Task<CardModel?> FromChooseACardRewardScreen(
        Player player,
        List<CardModel> options,
        bool canSkip = true
    )
    {
        var chosenCard = await CardSelectCmd.FromChooseACardScreen(
            new BlockingPlayerChoiceContext(),
            options,
            player,
            canSkip: true
        );
        if (chosenCard != null)
            CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(chosenCard, PileType.Deck));
        foreach (var card in options.Where(c => c != chosenCard))
            player
                .RunState.CurrentMapPointHistoryEntry?.GetEntry(player.NetId)
                .CardChoices.Add(new CardChoiceHistoryEntry(card, false));
        return chosenCard;
    }

    public static async Task<CardModel?> FromSelectACardScreenIfPossible(
        PlayerChoiceContext choiceContext,
        IReadOnlyList<CardModel> cards,
        Player player,
        LocString prompt,
        bool canSkip = false
    )
    {
        if (cards.Count == 0)
            return null;
        if (cards.Count == 1 && !canSkip)
            return cards[0];
        if (cards.Count <= 3)
            return await CardSelectCmd.FromChooseACardScreen(choiceContext, cards, player, canSkip);

        var selected = await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            cards,
            player,
            new CardSelectorPrefs(prompt, minCount: canSkip ? 0 : 1, maxCount: 1)
        );
        return selected.FirstOrDefault();
    }

    // Some base CardSelectCmds don't support custom filters. They're also very wet.
    // We refactor the comon part out here to implement our own custom filter-including implementations.
    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    private static async Task<IEnumerable<CardModel>> CustomDeckSelection(
        Player player,
        CardSelectorPrefs prefs,
        Func<CardModel, bool> filter,
        Func<List<CardModel>, Task<IEnumerable<CardModel>>> defaultLocalSelector
    )
    {
        List<CardModel> list =
        [
            .. PileType
                .Deck.GetPile(player)
                .Cards.Where(c => c.Type != CardType.Quest && c.IsTransformable && filter(c)),
        ];
        if (list.Count == 0)
            return [];
        IEnumerable<CardModel> cardModels;
        if (list.Count <= prefs.MinSelect && !prefs.RequireManualConfirmation)
            cardModels = list;
        else if (CardSelectCmd.Selector != null)
            cardModels = await CardSelectCmd.Selector.GetSelectedCards(
                list,
                prefs.MinSelect,
                prefs.MaxSelect
            );
        else
        {
            var choiceId = RunManager.Instance.PlayerChoiceSynchronizer.ReserveChoiceId(player);
            if (CardSelectCmd.ShouldSelectLocalCard(player))
            {
                if (CardSelectCmd.LocalSelector != null)
                    cardModels = await CardSelectCmd.LocalSelector.GetSelectedCards(
                        list,
                        prefs.MinSelect,
                        prefs.MaxSelect
                    );
                else
                {
                    cardModels = await defaultLocalSelector(list);
                    RunManager.Instance.PlayerChoiceSynchronizer.SyncLocalChoice(
                        player,
                        choiceId,
                        PlayerChoiceResult.FromMutableDeckCards(cardModels)
                    );
                }
            }
            else
                cardModels = (
                    await RunManager.Instance.PlayerChoiceSynchronizer.WaitForRemoteChoice(
                        player,
                        choiceId
                    )
                ).AsDeckCards();
        }
        CardSelectCmd.LogChoice(player, cardModels);
        return cardModels;
    }

    public static Task<IEnumerable<CardModel>> FromDeckForTransformationWithFilter(
        Player player,
        CardSelectorPrefs prefs,
        Func<CardModel, bool> filter,
        Func<CardModel, CardTransformation>? cardToTransformation = null
    ) =>
        CustomDeckSelection(
            player,
            prefs,
            filter,
            async (cards) =>
            {
                cardToTransformation ??= c => new CardTransformation(c);
                return await NDeckTransformSelectScreen
                    .ShowScreen(cards, cardToTransformation, prefs)
                    .CardsSelected();
            }
        );

    public static async Task<
        IEnumerable<CardPileAddResult>
    > FromDeckForTransformationPreservingMutations<T>(
        Player player,
        CardSelectorPrefs prefs,
        Func<CardModel, bool> filter
    )
        where T : CardModel
    {
        var cards = await FromDeckForTransformationWithFilter(
            player,
            prefs,
            filter,
            c => new CardTransformation(c, TransformPreservingMutations<T>(c, preview: true))
        );
        List<CardPileAddResult> transformed =
        [
            .. await CardCmd.Transform(
                cards.Select(c => new CardTransformation(
                    c,
                    TransformPreservingMutations<T>(c, preview: false)
                )),
                player.PlayerRng.Transformations
            ),
        ];
        CardCmd.PreviewCardPileAdd(transformed);
        return transformed;
    }

    private static T TransformPreservingMutations<T>(CardModel orig, bool preview)
        where T : CardModel
    {
        var player = orig.Owner;
        var card = preview
            ? (T)ModelDb.Card<T>().ToMutable()
            : player.RunState.CreateCard<T>(player);
        if (orig.IsUpgraded && card.IsUpgradable)
        {
            if (preview)
                card.UpgradeInternal();
            else
                CardCmd.Upgrade(card);
        }

        if (orig.Enchantment is not { } enchantment)
            return card;
        enchantment = (EnchantmentModel)enchantment.MutableClone();
        if (!enchantment.CanEnchant(card))
            return card;

        if (preview)
        {
            card.EnchantInternal(enchantment, enchantment.Amount);
            enchantment.ModifyCard();
        }
        else
            CardCmd.Enchant(enchantment, card, enchantment.Amount);
        return card;
    }

    public static Task<IEnumerable<CardModel>> FromDeckForUpgradeWithFilter(
        Player player,
        CardSelectorPrefs prefs,
        Func<CardModel, bool> filter
    ) =>
        CustomDeckSelection(
            player,
            prefs,
            filter,
            cards =>
                NDeckUpgradeSelectScreen.ShowScreen(cards, prefs, player.RunState).CardsSelected()
        );
}
