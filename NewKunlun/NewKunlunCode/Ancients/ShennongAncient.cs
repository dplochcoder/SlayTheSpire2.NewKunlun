using MegaCrit.Sts2.Core.Events;
using NewKunlun.NewKunlunCode.Relics;

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
