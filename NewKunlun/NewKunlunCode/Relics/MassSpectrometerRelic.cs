using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Mass Spectrometer",
    description: "Whenever you receive a potion reward, receive a linked alternate potion reward.",
    flavor: ""
)]
public class MassSpectrometerRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    private bool ModifyReward(Reward reward, out Reward newReward)
    {
        List<PotionReward> potionRewards = [];
        switch (reward)
        {
            case PotionReward potionReward:
                potionRewards.Add(potionReward);
                break;
            case LinkedRewardSet set when set.Rewards.All(r => r is PotionReward):
                potionRewards.AddRange(set.Rewards.OfType<PotionReward>());
                break;
        }

        if (potionRewards.Count == 0)
        {
            newReward = reward;
            return false;
        }

        HashSet<ModelId> forbidden = [.. potionRewards.Select(r => r.Potion!.Id!)];
        List<PotionModel> pool = [.. PotionFactory.GetPotionOptions(Owner)];
        if (!pool.All(p => forbidden.Contains(p.Id)))
            pool.RemoveAll(p => forbidden.Contains(p.Id));

        potionRewards.AddRange(
            PotionFactory
                .CreateRandomPotions(pool, 1, Owner.RunState.Rng.CombatPotionGeneration)
                .Select(p => new PotionReward(p, reward.Player))
        );
        newReward = new LinkedRewardSet([.. potionRewards], reward.Player);
        return true;
    }

    public override bool TryModifyRewards(Player player, List<Reward> rewards, AbstractRoom? room)
    {
        if (player != Owner)
            return false;

        var modified = false;
        for (var i = 0; i < rewards.Count; i++)
        {
            if (!ModifyReward(rewards[i], out var newReward))
                continue;

            rewards[i] = newReward;
            modified = true;
        }
        return modified;
    }
}
