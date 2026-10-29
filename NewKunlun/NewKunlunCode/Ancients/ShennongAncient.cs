using MegaCrit.Sts2.Core.Events;
using NewKunlun.NewKunlunCode.Relics;

namespace NewKunlun.NewKunlunCode.Ancients;

public class ShennongAncient : NewKunlunAncient
{
    protected override List<List<EventOption>> PositiveOptions =>
        [
            [RelicOption<FermentationKitRelic>()],
            [RelicOption<PoisonDartsRelic>()],
        ];

    protected override List<EventOption> CurseOptions => [RelicOption<OrpimentRelic>()];
}
