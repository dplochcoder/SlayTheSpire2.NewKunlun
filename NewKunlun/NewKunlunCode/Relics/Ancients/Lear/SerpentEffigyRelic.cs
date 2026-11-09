using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Enchantments;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Lear;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Serpent Effigy",
    description: "Enchant up to {NumCards} cards with [gold]Limitless[/gold].",
    flavor: ""
)]
public partial class SerpentEffigyRelic : EnchantCardsRelic<LimitlessEnchantment>
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new(nameof(NumCards), 3)];

    protected override int NumCardsMax => NumCards.IntValue;
}
