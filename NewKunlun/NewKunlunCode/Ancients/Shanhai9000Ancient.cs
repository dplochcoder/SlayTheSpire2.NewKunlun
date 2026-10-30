using MegaCrit.Sts2.Core.Events;
using NewKunlun.NewKunlunCode.Relics;

namespace NewKunlun.NewKunlunCode.Ancients;

public class Shanhai9000Ancient : NewKunlunAncient
{
    protected override List<List<EventOption>> PositiveOptions =>
        [
            [RelicOption<JinProcessorRelic>()],
        ];

    protected override List<EventOption> CurseOptions => [];
}
