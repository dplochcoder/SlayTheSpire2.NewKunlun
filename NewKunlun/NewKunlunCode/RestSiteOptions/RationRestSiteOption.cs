using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Factories;

namespace NewKunlun.NewKunlunCode.RestSiteOptions;

public class RationRestSiteOption(Player owner) : RestSiteOption(owner)
{
    public override string OptionId => "NEW_KUNLUN-RATION";

    public override async Task<bool> OnSelect()
    {
        await PlayerCmd.GainMaxPotionCount(1, Owner);
        while (Owner.HasOpenPotionSlots)
            await PotionCmd.TryToProcure(
                PotionFactory.CreateRandomPotionOutOfCombat(
                    Owner,
                    Owner.RunState.Rng.CombatPotionGeneration
                ),
                Owner
            );
        return true;
    }
}
