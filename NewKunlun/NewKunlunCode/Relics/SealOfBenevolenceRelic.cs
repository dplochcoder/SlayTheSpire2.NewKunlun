using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

// TODO
[Pool(typeof(YiRelicPool))]
[RelicLocalization(title: "Seal of Benevolence", description: "", flavor: "")]
public class SealOfBenevolenceRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Event;
}
