using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Enchantments;

namespace NewKunlun.NewKunlunCode.Commands;

public static class CardEnchantCmd
{
    public static async Task EnchantCardsFromDeck<T>(Player owner, int amount, int min, int max)
        where T : EnchantmentModel
    {
        CardSelectorPrefs prefs = new(
            CardSelectorPrefs.EnchantSelectionPrompt,
            minCount: min,
            maxCount: max
        )
        {
            Cancelable = false,
            RequireManualConfirmation = true,
        };
        foreach (
            var card in await CardSelectCmd.FromDeckForEnchantment(
                owner,
                ModelDb.Enchantment<T>(),
                amount,
                prefs
            )
        )
        {
            CardCmd.Enchant<AmortizedEnchantment>(card, amount);
            CardCmd.Preview(card);
        }
    }
}
