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
    title: "Poison Darts",
    description: "Enchant up to 3 {Cards} attacks with [gold]Venomous[/gold] {Venomous}.",
    flavor: ""
)]
public partial class PoisonDartsRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new(nameof(Cards), 3), new(nameof(Venomous), 4)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        HoverTipFactory.FromEnchantment<VenomousEnchantment>();

    public override bool IsAllowedForPlayer(Player player) =>
        player.Deck.Cards.Count(ModelDb.Enchantment<VenomousEnchantment>().CanEnchant)
        >= Cards.IntValue;

    public override bool HasUponPickupEffect => true;

    public override Task AfterObtained() =>
        ExtraCardSelectCmds.EnchantFromDeck<VenomousEnchantment>(
            Owner,
            Venomous.IntValue,
            0,
            Cards.IntValue
        );
}
