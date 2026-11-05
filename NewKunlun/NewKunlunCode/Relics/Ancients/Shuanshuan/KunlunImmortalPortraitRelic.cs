using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Enchantments;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Shuanshuan;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Kunlun Immortal Portrait",
    description: "Enchant a Power in your deck with [gold]Immortal[/gold].",
    flavor: ""
)]
public class KunlunImmortalPortraitRelic : EnchantCardsRelic<ImmortalEnchantment>
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override int NumCardsMin => 1;

    protected override int NumCardsMax => 1;
}
