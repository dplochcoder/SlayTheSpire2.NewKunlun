using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Enchantments;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Shuanshuan;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Multi Tool",
    description: "On pickup, enchant {Cards} cards with [gold]Multi Tool[/gold].",
    flavor: ""
)]
public partial class MultiToolKitRelic : EnchantCardsRelic<MultiToolEnchantment>
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new(nameof(Cards), 3)];

    protected override int NumCardsMin => Cards.IntValue;

    protected override int NumCardsMax => Cards.IntValue;
}
