using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Seals;

// TODO
[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Seal of Integrity",
    description: "Unlocks the Sol of Fuxi at the end of the run.",
    flavor: ""
)]
public class SealOfIntegrityRelic : SealRelic
{
    public override RelicRarity Rarity => RelicRarity.Event;
}
