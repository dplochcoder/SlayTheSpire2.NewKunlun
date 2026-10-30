using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

// TODO
[Pool(typeof(SharedRelicPool))]
[RelicLocalization(title: "Seal of Equality", description: "", flavor: "")]
public class SealOfEqualityRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Event;
}
