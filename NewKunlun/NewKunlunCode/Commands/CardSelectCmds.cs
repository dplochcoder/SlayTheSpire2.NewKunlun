using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

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
}
