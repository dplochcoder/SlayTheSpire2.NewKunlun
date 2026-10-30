using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Relics;

// TODO
[Pool(typeof(SharedRelicPool))]
[RelicLocalization(title: "Seal of Insight", description: "", flavor: "")]
public class SealOfInsightRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Event;
}
