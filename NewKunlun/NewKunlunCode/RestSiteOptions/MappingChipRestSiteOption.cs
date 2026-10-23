using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using NewKunlun.NewKunlunCode.Relics;

namespace NewKunlun.NewKunlunCode.RestSiteOptions;

public class MappingChipRestSiteOption(Player owner) : RestSiteOption(owner)
{
    public override string OptionId => "NEW_KUNLUN-MAPPING_CHIP";

    public override bool IsEnabled => Owner.Gold >= MappingChipRelic.GoldCost;

    public override async Task<bool> OnSelect()
    {
        await PlayerCmd.LoseGold(MappingChipRelic.GoldCost, Owner, GoldLossType.Spent);
        Owner.Relics.OfType<MappingChipRelic>().FirstOrDefault()?.UsesRemaining += 2;
        return true;
    }

    // TODO: Vfx
}
