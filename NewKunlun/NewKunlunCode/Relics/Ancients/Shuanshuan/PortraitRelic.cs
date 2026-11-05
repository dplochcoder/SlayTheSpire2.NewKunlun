using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Shuanshuan;

public abstract class PortraitRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool HasUponPickupEffect => true;

    private CardReward ForRarity(CardRarity rarity)
    {
        CardReward reward = new(
            new CardCreationOptions(
                [ModelDb.CardPool<ColorlessCardPool>()],
                CardCreationSource.Other,
                CardRarityOddsType.Uniform,
                c => c.Rarity == rarity
            ).WithFlags(
                CardCreationFlags.NoRarityModification | CardCreationFlags.NoCardPoolModifications
            ),
            3,
            Owner
        );
        reward.AfterGenerated += UpgradeCards;
        return reward;

        void UpgradeCards()
        {
            foreach (
                var card in reward.Cards.Where(c => c is { IsUpgradable: true, IsUpgraded: false })
            )
                CardCmd.Upgrade(card);
        }
    }

    public override Task AfterObtained()
    {
        RewardsSet set = new(Owner);
        set.Rewards.Add(ForRarity(CardRarity.Common));
        set.Rewards.Add(ForRarity(CardRarity.Uncommon));
        set.Rewards.Add(ForRarity(CardRarity.Rare));
        return set.Offer();
    }
}
