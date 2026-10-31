using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Localization;

// TODO
namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(title: "Seal of Integrity", description: "", flavor: "")]
public class SealOfIntegrityRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Event;
}
