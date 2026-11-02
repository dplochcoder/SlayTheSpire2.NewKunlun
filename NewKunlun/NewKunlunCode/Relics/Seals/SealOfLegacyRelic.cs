using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Enchantments;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Seals;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Seal of Legacy",
    description: "Enchant up to {Cards} cards with [gold]Antiquated[/gold].",
    flavor: ""
)]
public partial class SealOfLegacyRelic : SealRelic
{
    public override RelicRarity Rarity => RelicRarity.Event;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new(nameof(Cards), 6)];

    public override bool IsAllowedForPlayer(Player player) =>
        player.Deck.Cards.Count(ModelDb.Enchantment<AntiquatedEnchantment>().CanEnchant) >= 6;

    public override bool HasUponPickupEffect => true;

    public override Task AfterObtained() =>
        ExtraCardSelectCmds.EnchantFromDeck<AntiquatedEnchantment>(Owner, 1, 0, Cards.IntValue);
}
