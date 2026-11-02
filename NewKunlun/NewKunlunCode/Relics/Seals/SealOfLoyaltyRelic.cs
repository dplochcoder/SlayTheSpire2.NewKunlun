using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Enchantments;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Seals;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Seal of Loyalty",
    description: "On pickup, enchant up to {Cards} cards with [gold]Loyal[/gold].",
    flavor: ""
)]
public partial class SealOfLoyaltyRelic : SealRelic
{
    public override RelicRarity Rarity => RelicRarity.Event;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new(nameof(Cards), 2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        HoverTipFactory.FromEnchantment<LoyalEnchantment>();

    public override bool IsAllowedForPlayer(Player player) =>
        player.Deck.Cards.Count(ModelDb.Enchantment<LoyalEnchantment>().CanEnchant)
        >= Cards.IntValue;

    public override bool HasUponPickupEffect => true;

    public override Task AfterObtained() =>
        ExtraCardSelectCmds.EnchantFromDeck<LoyalEnchantment>(Owner, 1, 0, Cards.IntValue);
}
