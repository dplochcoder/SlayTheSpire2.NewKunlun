using System.Diagnostics.CodeAnalysis;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using MegaCrit.Sts2.Core.Runs;

namespace NewKunlun.NewKunlunCode.Commands;

public static class CardSelectCmds
{
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

    // FromDeckForUpgrade doesn't support filtering, so we rewrite it.
    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    public static async Task<IEnumerable<CardModel>> FromDeckForUpgradeWithFilter(
        Player player,
        CardSelectorPrefs prefs,
        Func<CardModel, bool> filter
    )
    {
        List<CardModel> list = [.. player.Deck.Cards.Where(c => c.IsUpgradable && filter(c))];
        if (list.Count == 0)
            return [];
        IEnumerable<CardModel> cardModels;
        if (list.Count <= prefs.MinSelect && !prefs.RequireManualConfirmation)
            cardModels = list;
        else if (CardSelectCmd.Selector != null)
        {
            cardModels = await CardSelectCmd.Selector.GetSelectedCards(
                list,
                prefs.MinSelect,
                prefs.MaxSelect
            );
        }
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
                    cardModels = await NDeckUpgradeSelectScreen
                        .ShowScreen(list, prefs, player.RunState)
                        .CardsSelected();
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
}
