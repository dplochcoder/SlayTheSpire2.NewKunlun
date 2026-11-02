using MegaCrit.Sts2.Core.Events;
using NewKunlun.NewKunlunCode.Relics.Ancients.Shennong;
using GutwrenchFruitRelic = NewKunlun.NewKunlunCode.Relics.Ancients.Shennong.GutwrenchFruitRelic;
using JiuGongRelic = NewKunlun.NewKunlunCode.Relics.Ancients.Shennong.JiuGongRelic;
using NecrocepsRelic = NewKunlun.NewKunlunCode.Relics.Ancients.Shennong.NecrocepsRelic;
using PoisonDartsRelic = NewKunlun.NewKunlunCode.Relics.Ancients.Shennong.PoisonDartsRelic;
using SpitwineRelic = NewKunlun.NewKunlunCode.Relics.Ancients.Shennong.SpitwineRelic;
using ThunderCentipedeRelic = NewKunlun.NewKunlunCode.Relics.Ancients.Shennong.ThunderCentipedeRelic;
using ZengZiRelic = NewKunlun.NewKunlunCode.Relics.Ancients.Shennong.ZengZiRelic;

namespace NewKunlun.NewKunlunCode.Ancients;

public class ShennongAncient : NewKunlunAncient
{
    protected override List<List<EventOption>> PositiveOptions =>
        [
            [
                RelicOption<JiuGongRelic>(),
                RelicOption<ZengZiRelic>(),
                RelicOption<GutwrenchFruitRelic>(),
            ],
            [
                RelicOption<DragonSnakeRelic>(),
                RelicOption<PoisonDartsRelic>(),
                RelicOption<ThunderCentipedeRelic>(),
            ],
        ];

    protected override List<EventOption> CurseOptions =>
        [RelicOption<NecrocepsRelic>(), RelicOption<OrpimentRelic>(), RelicOption<SpitwineRelic>()];
}
