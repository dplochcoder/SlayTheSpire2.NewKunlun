using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Abacus;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Bloody Crimson Hibiscus",
    description: "On pickup, add one [gold]Doubt[/gold] and one [gold]Injury[/gold] to your deck.\nDraw 1 additional card each turn.",
    flavor: ""
)]
public class BloodyCrimsonHibiscus : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<Doubt>(), HoverTipFactory.FromCard<Injury>()];

    public override bool HasUponPickupEffect => true;

    public override Task AfterObtained() =>
        Owner.AddCardsToDeck([ModelDb.Card<Doubt>(), ModelDb.Card<Injury>()]);

    public override decimal ModifyHandDraw(Player player, decimal count) =>
        player == Owner ? count + 1 : count;
}
