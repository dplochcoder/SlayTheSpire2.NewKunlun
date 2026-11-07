using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using NewKunlun.NewKunlunCode.Relics.Ancients.Lear;

namespace NewKunlun.NewKunlunCode.Ancients;

public class LearAncient : NewKunlunAncient
{
    protected override List<List<EventOption>> PositiveOptions =>
        [
            [RelicOption<AdamantiumRelic>(), RelicOption<InactionDeclarationRelic>()],
            [RelicOption<CelestialPasskeyRelic>()],
            [
                RelicOption<FusangCoreRelic>(),
                RelicOption<FusangHornRelic>(),
                RelicOption<GorgeousFlowerRelic>(),
            ],
            [
                RelicOption<FangshiMaskRelic>(),
                RelicOption<HaotianTelescopeRelic>(),
                RelicOption<LimitlessBladeRelic>(),
            ],
            [RelicOption<PeaceBringerRelic>()],
            [RelicOption<TrainingWeightsRelic>()],
        ];

    protected override List<EventOption> CurseOptions =>
        [
            RelicOption<DemonsBladeRelic>(),
            RelicOption<FangshuBeltRelic>(),
            RelicOption<FangshiSealRelic>(),
            RelicOption<FusangWheelRelic>(),
            RelicOption<LimitlessCloakRelic>(),
            RelicOption<PrimordialRootRelic>(),
        ];
}
