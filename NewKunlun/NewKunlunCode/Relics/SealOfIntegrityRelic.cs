using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Relics;

// TODO
[Pool(typeof(YiRelicPool))]
[RelicLocalization(title: "Seal of Integrity", description: "", flavor: "")]
public class SealOfIntegrityRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Event;
}
