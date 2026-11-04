using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Runs;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Shuanshuan;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "GM Fertilizer",
    description: "All your card rewards are enchanted with [gold]Sown[/gold].",
    flavor: ""
)]
public class GmFertilizerRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        HoverTipFactory.FromEnchantment<Sown>();

    public override bool TryModifyCardRewardOptionsLate(
        Player player,
        List<CardCreationResult> cardRewards,
        CardCreationOptions options
    )
    {
        if (player != Owner || !options.Flags.HasFlag(CardCreationFlags.IsCardReward))
            return false;

        foreach (var cardReward in cardRewards)
        {
            var sown = ModelDb.Enchantment<Sown>();
            if (!sown.CanEnchant(cardReward.Card))
                continue;

            var clone = cardReward.Card.CreateClone();
            CardCmd.Enchant<Sown>(clone, 1);
            cardReward.ModifyCard(clone, this);
        }
        return true;
    }
}
