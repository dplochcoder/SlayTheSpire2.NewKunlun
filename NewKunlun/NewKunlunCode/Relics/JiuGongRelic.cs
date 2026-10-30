using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Potions;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Jiu-gong",
    description: "On pickup, gain {PotionSlots} potion slots and {Potions} [gold]Entropic Brew[/gold].",
    flavor: ""
)]
public partial class JiuGongRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new(nameof(PotionSlots), 3), new(nameof(Potions), 2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPotion<EntropicBrew>()];

    public override async Task AfterObtained()
    {
        await PlayerCmd.GainMaxPotionCount(PotionSlots.IntValue, Owner);
        for (var i = 0; i < Potions.IntValue; i++)
            await PotionCmd.TryToProcure<EntropicBrew>(Owner);
    }
}
