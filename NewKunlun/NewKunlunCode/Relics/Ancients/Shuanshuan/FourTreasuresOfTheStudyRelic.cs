using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Enchantments;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Shuanshuan;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Four Treasures of the Study",
    description: "Enchant {Cards} cards with [gold]Calligraphic[/gold] {CalligraphicAmount}.",
    flavor: ""
)]
public partial class FourTreasuresOfTheStudyRelic : EnchantCardsRelic<CalligraphicEnchantment>
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new(nameof(Cards), 4), new(nameof(CalligraphicAmount), 2)];

    protected override int EnchantAmount => CalligraphicAmount.IntValue;

    protected override int NumCardsMin => Cards.IntValue;

    protected override int NumCardsMax => Cards.IntValue;
}
