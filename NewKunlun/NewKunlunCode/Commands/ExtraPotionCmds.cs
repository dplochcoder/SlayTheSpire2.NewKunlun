using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;

namespace NewKunlun.NewKunlunCode.Commands;

public static class ExtraPotionCmds
{
    public static async Task ProcureOrOffer<T>(Player player)
        where T : PotionModel
    {
        var result = await PotionCmd.TryToProcure<T>(player);
        if (!result.success && result.failureReason == PotionProcureFailureReason.TooFull)
        {
            RewardsSet set = new(player);
            set.Rewards.Add(new PotionReward(ModelDb.Potion<T>(), player));
            await set.Offer();
        }
    }
}
