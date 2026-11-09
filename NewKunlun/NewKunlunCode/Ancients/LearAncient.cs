using MegaCrit.Sts2.Core.Events;
using NewKunlun.NewKunlunCode.Relics.Ancients.Lear;

namespace NewKunlun.NewKunlunCode.Ancients;

public class LearAncient : NewKunlunAncient
{
    protected override List<List<EventOption>> PositiveOptions =>
        [
            [
                RelicOption<AdamantiumRelic>(),
                RelicOption<InactionDeclarationRelic>(),
                RelicOption<SerpentEffigyRelic>(),
            ],
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
                RelicOption<FoxEffigyRelic>(),
            ],
            [RelicOption<PeaceBringerRelic>(), RelicOption<TigerEffigyRelic>()],
            [RelicOption<TrainingWeightsRelic>()],
        ];

    protected override List<EventOption> CurseOptions =>
        [
            RelicOption<FangshuBeltRelic>(),
            RelicOption<FangshiSealRelic>(),
            RelicOption<FusangWheelRelic>(),
            RelicOption<LimitlessCloakRelic>(),
            RelicOption<PrimordialRootRelic>(),
        ];
}
