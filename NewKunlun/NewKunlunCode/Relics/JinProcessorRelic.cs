using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Cards;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using ReconstructCard = NewKunlun.NewKunlunCode.Cards.Ancients.ReconstructCard;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Jin Processor",
    description: "Add one [gold]Reconstruct[/gold] to your deck.",
    flavor: ""
)]
public class JinProcessorRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<ReconstructCard>()];

    public override bool HasUponPickupEffect => true;

    public override Task AfterObtained() => Owner.AddCardToDeck<ReconstructCard>();
}
