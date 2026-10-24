using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Enchantments;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(YiRelicPool))]
[RelicLocalization(
    title: "Multi Tool",
    description: "On pickup, enchant {Cards} cards with [gold]Multi Tool[/gold].",
    flavor: ""
)]
public partial class MultiToolRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar(nameof(Cards), 3)];

    public override bool HasUponPickupEffect => true;

    public override Task AfterObtained() =>
        CardEnchantCmd.EnchantCardsFromDeck<MultiToolEnchantment>(Owner, 1, 0, Cards.IntValue);
}
