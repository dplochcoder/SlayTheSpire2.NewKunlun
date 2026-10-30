using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Potions;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Emergency Kit",
    description: "On pickup, gain 2 [gold]Flash Bombs[/gold].",
    flavor: ""
)]
public class EmergencyKitRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPotion<FlashBombPotion>()];

    public override bool HasUponPickupEffect => true;

    public override async Task AfterObtained()
    {
        await PotionCmd.TryToProcure<FlashBombPotion>(Owner);
        await PotionCmd.TryToProcure<FlashBombPotion>(Owner);
    }
}
