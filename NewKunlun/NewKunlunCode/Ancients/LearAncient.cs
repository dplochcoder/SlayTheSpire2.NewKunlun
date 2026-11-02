using MegaCrit.Sts2.Core.Events;
using NewKunlun.NewKunlunCode.Relics.Ancients.Lear;
using InactionDeclarationRelic = NewKunlun.NewKunlunCode.Relics.Ancients.Lear.InactionDeclarationRelic;
using LimitlessCloakRelic = NewKunlun.NewKunlunCode.Relics.Ancients.Lear.LimitlessCloakRelic;

namespace NewKunlun.NewKunlunCode.Ancients;

public class LearAncient : NewKunlunAncient
{
    protected override List<List<EventOption>> PositiveOptions =>
        [
            [RelicOption<FusangHornRelic>()],
            [RelicOption<InactionDeclarationRelic>()],
            [RelicOption<LearsBladeRelic>(), RelicOption<LearsMaskRelic>()],
            [RelicOption<PeaceBringerRelic>()],
            [RelicOption<TrainingWeightsRelic>()],
        ];

    protected override List<EventOption> CurseOptions =>
        [
            RelicOption<FangshuBeltRelic>(),
            RelicOption<FangshiSealRelic>(),
            RelicOption<LimitlessCloakRelic>(),
        ];
}
