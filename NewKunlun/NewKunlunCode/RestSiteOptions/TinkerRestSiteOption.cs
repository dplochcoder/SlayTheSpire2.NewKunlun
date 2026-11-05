using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Rewards;
using NewKunlun.NewKunlunCode.Rewards;

namespace NewKunlun.NewKunlunCode.RestSiteOptions;

public class TinkerRestSiteOption(Player owner) : RestSiteOption(owner)
{
    public override string OptionId => "NEW_KUNLUN-TINKER";

    private IReadOnlyList<CardRarity> ValidRarities()
    {
        List<CardRarity> rarities = [CardRarity.Basic, CardRarity.Common, CardRarity.Uncommon];
        return
        [
            .. rarities.Where(r =>
                Owner.Deck.Cards.Count(c => c.Rarity == r && c.IsRemovable) >= 2
            ),
        ];
    }

    public override bool IsEnabled => ValidRarities().Count > 0;

    public override async Task<bool> OnSelect()
    {
        var rarities = ValidRarities();
        switch (rarities.Count)
        {
            case 0:
                return false;
            case 1:
                return await TinkerReward.Tinker(Owner, rarities[0]);
            default:
                RewardsSet set = new(Owner);
                LinkedRewardSet reward = new(
                    [.. rarities.Select(r => new TinkerReward(Owner, r))],
                    Owner
                );
                set.Rewards.Add(reward);
                await set.Offer();
                return set.AllRewardsSuccessfullySelected;
        }
    }
}
