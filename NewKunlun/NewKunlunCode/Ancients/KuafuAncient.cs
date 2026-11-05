using MegaCrit.Sts2.Core.Events;
using NewKunlun.NewKunlunCode.Relics.Ancients.Kuafu;

namespace NewKunlun.NewKunlunCode.Ancients;

public class KuafuAncient : NewKunlunAncient
{
    protected override List<List<EventOption>> PositiveOptions =>
        [
            [
                RelicOption<BubbleTeaRelic>(),
                RelicOption<FloatationDeviceRelic>(),
                RelicOption<SolarPanelRelic>(),
            ],
            [RelicOption<Printer3DRelic>(), RelicOption<ToolbeltRelic>()],
            [RelicOption<RhizomaticSharpenerRelic>()],
        ];

    protected override List<EventOption> CurseOptions => [RelicOption<RhizomaticBatteryRelic>()];
}
