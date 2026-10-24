using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using NewKunlun.NewKunlunCode.Cards;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(YiRelicPool))]
[RelicLocalization(
    title: "Locked Chest",
    description: "On pickup, add 1 [gold]Hack[/gold] to your deck.",
    flavor: ""
)]
public class LockedChestRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.Card<HackCard>()];

    public override bool HasUponPickupEffect => true;

    public override Task AfterObtained() => Owner.AddCardToDeck<HackCard>();
}
