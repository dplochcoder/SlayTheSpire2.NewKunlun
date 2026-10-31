using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Enchantments;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Balanced Blade",
    description: "On pickup, enchant an Attack with [gold]Precise[/gold].",
    flavor: ""
)]
public class BalancedBladeRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        HoverTipFactory.FromEnchantment<PreciseEnchantment>();

    public override bool HasUponPickupEffect => true;

    public override Task AfterObtained() =>
        ExtraCardSelectCmds.EnchantFromDeck<PreciseEnchantment>(Owner, 1, 0, 1);
}
