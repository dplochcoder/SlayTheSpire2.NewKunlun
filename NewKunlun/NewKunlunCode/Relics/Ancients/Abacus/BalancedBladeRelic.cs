using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Enchantments;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Abacus;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Balanced Blade",
    description: "On pickup, enchant an Attack with [gold]Precise[/gold].",
    flavor: ""
)]
public class BalancedBladeRelic : EnchantCardsRelic<PreciseEnchantment>
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override int NumCardsMin => 1;

    protected override int NumCardsMax => 1;
}
