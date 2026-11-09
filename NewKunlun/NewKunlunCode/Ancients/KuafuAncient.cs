using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Relics.Ancients.Kuafu;

namespace NewKunlun.NewKunlunCode.Ancients;

public class KuafuAncient : NewKunlunAncient
{
    private IEnumerable<EventOption> WorkbenchOption()
    {
        if (Owner == null || WorkbenchRelic.ForPlayer(Owner) is not { } relic)
            return [];
        else
            return [RelicOption(relic)];
    }

    protected override List<List<EventOption>> PositiveOptions =>
        [
            [
                RelicOption<BubbleTeaRelic>(),
                RelicOption<MiniPagodaRelic>(),
                RelicOption<SolarPanelRelic>(),
            ],
            [
                RelicOption<TuneUpRelic>(),
                RelicOption<Printer3DRelic>(),
                RelicOption<ToolbeltRelic>(),
            ],
            [RelicOption<FloatationDeviceRelic>()],
            [RelicOption<RhizomaticSharpenerRelic>(), .. WorkbenchOption()],
        ];

    protected override List<EventOption> CurseOptions =>
        [
            RelicOption<CompactorRelic>(),
            RelicOption<RhizomaticBatteryRelic>(),
            RelicOption<RhizomaticColumnRelic>(),
            RelicOption<SecurityCameraRelic>(),
        ];

    protected override bool IsCurseAllowed(
        RelicModel curse,
        IReadOnlyCollection<RelicModel> positive
    ) =>
        curse switch
        {
            CompactorRelic => !positive.Any(r => r is FloatationDeviceRelic),
            RhizomaticBatteryRelic or RhizomaticColumnRelic => !positive.Any(r =>
                r is BubbleTeaRelic or MiniPagodaRelic or SolarPanelRelic
            ),
            _ => true,
        };
}
