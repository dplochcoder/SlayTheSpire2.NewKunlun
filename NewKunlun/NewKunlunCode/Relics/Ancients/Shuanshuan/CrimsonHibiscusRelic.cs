using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Enchantments;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Shuanshuan;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Crimson Hibiscus",
    description: "On pickup, enchant 1 non-exhausting Attack or Skill in your deck with [gold]Blossom[/gold].",
    flavor: ""
)]
public class CrimsonHibiscusRelic : EnchantCardsRelic<BlossomEnchantment>
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override int NumCardsMax => 1;
}
