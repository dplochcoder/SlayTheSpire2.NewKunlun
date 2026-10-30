using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Relics;

// TODO
[Pool(typeof(SharedRelicPool))]
[RelicLocalization(title: "Seal of Selflessness", description: "", flavor: "")]
public class SealOfSelflessnessRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Event;
}
