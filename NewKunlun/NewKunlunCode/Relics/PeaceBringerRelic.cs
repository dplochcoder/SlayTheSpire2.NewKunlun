using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Potions;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Peace Bringer",
    description: "Gain 1 [gold]Rhizomatic Bomb[/gold].",
    flavor: ""
)]
public class PeaceBringerRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPotion<RhizomaticBombPotion>()];

    public override bool HasUponPickupEffect => true;

    public override Task AfterObtained() =>
        ExtraPotionCmds.ProcureOrOffer<RhizomaticBombPotion>(Owner);
}
