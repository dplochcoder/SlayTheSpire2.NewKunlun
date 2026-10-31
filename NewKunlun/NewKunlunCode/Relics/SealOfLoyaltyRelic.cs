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

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Seal of Loyalty",
    description: "Enchant up to {Cards} cards in your deck with [gold]Loyal[/gold].",
    flavor: ""
)]
public partial class SealOfLoyaltyRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new(nameof(Cards), 3)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        HoverTipFactory.FromEnchantment<LoyalEnchantment>();

    public override bool IsAllowedForPlayer(Player player) =>
        player.Deck.Cards.Count(ModelDb.Enchantment<LoyalEnchantment>().CanEnchant)
        >= Cards.IntValue;

    public override bool HasUponPickupEffect => true;

    public override Task AfterObtained() =>
        ExtraCardSelectCmds.EnchantFromDeck<LoyalEnchantment>(Owner, 1, 0, Cards.IntValue);
}
