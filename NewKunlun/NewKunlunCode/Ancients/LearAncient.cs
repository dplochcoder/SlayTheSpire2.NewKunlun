using MegaCrit.Sts2.Core.Events;
using NewKunlun.NewKunlunCode.Relics;

namespace NewKunlun.NewKunlunCode.Ancients;

public class LearAncient : NewKunlunAncient
{
    protected override List<List<EventOption>> PositiveOptions =>
        [
            [RelicOption<FusangHornRelic>()],
            [RelicOption<InactionDeclarationRelic>()],
            [RelicOption<PeaceBringerRelic>()],
            [RelicOption<TrainingWeightsRelic>()],
        ];

    protected override List<EventOption> CurseOptions =>
        [RelicOption<FangshiSealRelic>(), RelicOption<LimitlessCloakRelic>()];
}
