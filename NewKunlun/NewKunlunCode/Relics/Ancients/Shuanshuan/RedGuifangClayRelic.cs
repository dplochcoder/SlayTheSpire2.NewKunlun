using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Cards.Ancients;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Shuanshuan;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Red Guifang Clay",
    description: "On pickup, add one [gold]Pottery Wheel[/gold] to your deck.",
    flavor: ""
)]
public class RedGuifangClayRelic : AddCardsToDeckRelic<PotteryWheelCard>
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override int NumCards => 1;
}
