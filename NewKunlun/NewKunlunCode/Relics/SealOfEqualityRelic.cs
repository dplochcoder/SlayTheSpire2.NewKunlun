using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

// TODO
[Pool(typeof(YiRelicPool))]
[RelicLocalization(title: "Seal of Equality", description: "", flavor: "")]
public class SealOfEqualityRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Event;
}
