using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using NewKunlun.NewKunlunCode.Relics;

namespace NewKunlun.NewKunlunCode.RestSiteOptions;

public class MetalDetectorRestSiteOption(Player owner) : RestSiteOption(owner)
{
    public override string OptionId => "NEW_KUNLUN-METAL_DETECTOR";

    public override async Task<bool> OnSelect()
    {
        var gold = Owner.PlayerRng.Rewards.NextInt(
            MetalDetectorRelic.GoldMinValue,
            MetalDetectorRelic.GoldMaxValue + 1
        );
        await PlayerCmd.GainGold(gold, Owner);
        return true;
    }
}
