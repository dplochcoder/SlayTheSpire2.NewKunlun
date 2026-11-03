using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Enchantments;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Seals;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Seal of Legacy",
    description: "Enchant up to {Cards} cards with [gold]Antiquated[/gold].",
    flavor: ""
)]
public partial class SealOfLegacyRelic
    : EnchantCardsRelic<AntiquatedEnchantment>,
        ISealRelic<SealOfLegacyRelic>
{
    public override RelicRarity Rarity => RelicRarity.Event;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new(nameof(Cards), 6)];

    protected override int NumCardsMax => Cards.IntValue;
}
