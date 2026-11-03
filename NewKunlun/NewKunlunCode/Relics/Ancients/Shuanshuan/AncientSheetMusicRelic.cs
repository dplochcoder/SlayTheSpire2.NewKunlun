using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Cards.Ancients;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Shuanshuan;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Ancient Sheet Music",
    description: "On pickup, add 4 [gold]Verses[/gold] to your deck.",
    flavor: ""
)]
public class AncientSheetMusicRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<VerseCard>(), HoverTipFactory.FromPower<QuarterNotePower>()];

    public override bool HasUponPickupEffect => true;

    public override Task AfterObtained() => Owner.AddCardsToDeck<VerseCard>(4);
}
