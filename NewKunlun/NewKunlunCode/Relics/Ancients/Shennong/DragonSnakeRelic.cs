using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Localization;
using DragonsWhipCard = NewKunlun.NewKunlunCode.Cards.Ancients.DragonsWhipCard;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Shennong;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Dragon Snake",
    description: "On pickup, add one [gold]Dragon's Whip[/gold] to your deck.",
    flavor: ""
)]
public class DragonSnakeRelic : AddCardsToDeckRelic<DragonsWhipCard>
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override int NumCards => 1;
}
