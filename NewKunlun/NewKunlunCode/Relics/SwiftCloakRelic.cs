using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Enchantments;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Swift Cloak",
    description: "On pickup, enchant a Skill with [gold]Poise[/gold].",
    flavor: ""
)]
public class SwiftCloakRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        HoverTipFactory.FromEnchantment<PoiseEnchantment>();

    public override bool HasUponPickupEffect => true;

    public override Task AfterObtained() =>
        CardEnchantCmd.EnchantCardsFromDeck<PoiseEnchantment>(Owner, 1, 0, 1);
}
