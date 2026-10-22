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
    title: "Seal of Legacy",
    description: "On pickup, enchant up to {Cards} cards with [gold]Antiquated[/gold].",
    flavor: ""
)]
public partial class SealOfLegacyRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Event;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar(nameof(Cards), 4)];

    public override bool HasUponPickupEffect => true;

    public override Task AfterObtained() =>
        CardEnchantCmd.EnchantCardsFromDeck<AntiquatedEnchantment>(Owner, 1, 0, Cards.IntValue);
}
