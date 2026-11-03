using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Enchantments;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Abacus;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Swift Cloak",
    description: "On pickup, enchant a Skill with [gold]Poise[/gold].",
    flavor: ""
)]
public class SwiftCloakRelic : EnchantCardsRelic<PoiseEnchantment>
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override int NumCardsMax => 1;
}
