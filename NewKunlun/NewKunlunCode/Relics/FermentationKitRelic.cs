using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(YiRelicPool))]
[RelicLocalization(
    title: "Fermentation Kit",
    description: "Add three [gold]Alchemize[/gold] to your deck.",
    flavor: ""
)]
public class FermentationKitRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.Card<Alchemize>()];

    public override bool HasUponPickupEffect => true;

    public override Task AfterObtained() => Owner.AddCardsToDeck<Alchemize>(3);
}
