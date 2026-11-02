using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Seals;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(title: "Seal of Selflessness", description: "", flavor: "")]
public class SealOfSelflessnessRelic : SealRelic
{
    public override RelicRarity Rarity => RelicRarity.Event;
    // TODO
}
