using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Shuanshuan;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Portrait of the Automaton",
    description: "On pickup, gain 3 upgraded Colorless card rewards.",
    flavor: ""
)]
public class PortraitOfTheAutomatonRelic : PortraitRelic { }
