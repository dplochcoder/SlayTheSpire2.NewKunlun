using MegaCrit.Sts2.Core.Events;
using NewKunlun.NewKunlunCode.Relics;

namespace NewKunlun.NewKunlunCode.Ancients;

public class KuafuAncient : NewKunlunAncient
{
    protected override List<List<EventOption>> PositiveOptions =>
        [
            [RelicOption<BubbleTeaRelic>(), RelicOption<SolarPanelRelic>()],
            [RelicOption<MultiToolRelic>(), RelicOption<SealOfLoyaltyRelic>()],
            [RelicOption<Printer3DRelic>()],
            [RelicOption<RhizomaticSharpenerRelic>()],
        ];

    protected override List<EventOption> CurseOptions => [RelicOption<RhizomaticBatteryRelic>()];
}
