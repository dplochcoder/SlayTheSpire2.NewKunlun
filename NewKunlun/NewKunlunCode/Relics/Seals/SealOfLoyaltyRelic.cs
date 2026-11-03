using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Enchantments;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Seals;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Seal of Loyalty",
    description: "On pickup, enchant up to {Cards} cards with [gold]Loyal[/gold].",
    flavor: ""
)]
public partial class SealOfLoyaltyRelic
    : EnchantCardsRelic<LoyalEnchantment>,
        ISealRelic<SealOfLoyaltyRelic>
{
    public override RelicRarity Rarity => RelicRarity.Event;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new(nameof(Cards), 2)];

    protected override int NumCardsMax => Cards.IntValue;
}
