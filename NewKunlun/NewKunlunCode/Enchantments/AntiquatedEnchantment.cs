using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Enchantments;

[EnchantmentLocalization(
    title: "Antiquated",
    description: "On any deck shuffle, this card is moved to the bottom of the deck.",
    extraCardText: "Antiquated."
)]
public class AntiquatedEnchantment : NewKunlunEnchantment
{
    public override bool HasExtraCardText => true;

    public override void ModifyShuffleOrder(
        Player player,
        List<CardModel> cards,
        bool isInitialShuffle
    )
    {
        if (!cards.Contains(Card))
            return;

        cards.Remove(Card);
        cards.Add(Card);
    }
}
