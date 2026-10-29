using MegaCrit.Sts2.Core.Events;
using NewKunlun.NewKunlunCode.Relics;

namespace NewKunlun.NewKunlunCode.Ancients;

public class KuafuAncient : NewKunlunAncient
{
    protected override List<List<EventOption>> PositiveOptions =>
        [
            [RelicOption<MultiToolRelic>()],
            [RelicOption<Printer3DRelic>()],
        ];

    protected override List<EventOption> CurseOptions => [RelicOption<RhizomaticBatteryRelic>()];
}
