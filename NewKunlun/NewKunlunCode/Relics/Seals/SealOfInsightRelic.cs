using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Seals;

// TODO
[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Unlocks the Sol of Ji at the end of the run.",
    description: "",
    flavor: ""
)]
public class SealOfInsightRelic : SealRelic
{
    public override RelicRarity Rarity => RelicRarity.Event;
}
