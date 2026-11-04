using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Enchantments;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Shennong;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Poison Darts",
    description: "Enchant up to {Cards} attacks with [gold]Venomous[/gold] {Venomous}.",
    flavor: ""
)]
public partial class PoisonDartsRelic : EnchantCardsRelic<VenomousEnchantment>
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new(nameof(Cards), 3), new(nameof(Venomous), 4)];

    protected override int EnchantAmount => Venomous.IntValue;

    protected override int NumCardsMax => Cards.IntValue;
}
