using MegaCrit.Sts2.Core.Events;
using NewKunlun.NewKunlunCode.Relics.Ancients.Kuafu;
using BubbleTeaRelic = NewKunlun.NewKunlunCode.Relics.Ancients.Kuafu.BubbleTeaRelic;
using MultiToolRelic = NewKunlun.NewKunlunCode.Relics.Ancients.Kuafu.MultiToolRelic;
using Printer3DRelic = NewKunlun.NewKunlunCode.Relics.Ancients.Kuafu.Printer3DRelic;
using SealOfLoyaltyRelic = NewKunlun.NewKunlunCode.Relics.Ancients.Kuafu.SealOfLoyaltyRelic;
using SolarPanelRelic = NewKunlun.NewKunlunCode.Relics.Ancients.Kuafu.SolarPanelRelic;

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
