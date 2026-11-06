using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Relics.Ancients.Lear;

namespace NewKunlun.NewKunlunCode.Ancients;

public class LearAncient : NewKunlunAncient
{
    protected override List<List<EventOption>> PositiveOptions =>
        [
            [RelicOption<AdamantiumRelic>(), RelicOption<InactionDeclarationRelic>()],
            [RelicOption<FusangCoreRelic>()],
            [RelicOption<FusangHornRelic>(), RelicOption<PottedPlantRelic>()],
            [
                RelicOption<HaotianTelescopeRelic>(),
                RelicOption<LearsBladeRelic>(),
                RelicOption<LearsMaskRelic>(),
            ],
            [RelicOption<PeaceBringerRelic>()],
            [RelicOption<TrainingWeightsRelic>()],
        ];

    protected override List<EventOption> CurseOptions =>
        [
            RelicOption<CelestialPasskeyRelic>(),
            RelicOption<FangshuBeltRelic>(),
            RelicOption<FangshiSealRelic>(),
            RelicOption<FusangWheelRelic>(),
            RelicOption<LimitlessCloakRelic>(),
        ];

    protected override bool IsCurseAllowed(
        RelicModel curse,
        IReadOnlyCollection<RelicModel> positive
    )
    {
        if (curse is CelestialPasskeyRelic)
            return !positive.Any(r => r is FusangHornRelic);

        return true;
    }
}
