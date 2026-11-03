using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Relics.Ancients.Abacus;

namespace NewKunlun.NewKunlunCode.Ancients;

public class AbacusAncient : NewKunlunAncient
{
    protected override List<List<EventOption>> PositiveOptions =>
        [
            [RelicOption<BalancedBladeRelic>(), RelicOption<SwiftCloakRelic>()],
            [RelicOption<ComputingUnitRelic>()],
            [RelicOption<DefenseMatrixRelic>(), RelicOption<FirewallRelic>()],
            [RelicOption<JietongHammerRelic>(), RelicOption<RhizomaticStabilizerRelic>()],
            [RelicOption<MapDataChipRelic>(), RelicOption<MetalDetectorRelic>()],
            [
                RelicOption<MassSpectrometerRelic>(),
                RelicOption<EmergencyKitRelic>(),
                RelicOption<QiBladeRelic>(),
            ],
            [RelicOption<MysticNymphRelic>(), RelicOption<ZenGardenRelic>()],
            [RelicOption<ScrapRelic>()],
            [RelicOption<SealOfInnovationRelic>()],
            [RelicOption<TaoFruitRelic>()],
        ];

    protected override List<EventOption> CurseOptions =>
        [
            RelicOption<CentrifugeRelic>(),
            RelicOption<FangshiStatueRelic>(),
            RelicOption<FusangRootRelic>(),
            RelicOption<LockedChestRelic>(),
            RelicOption<ParallelProcessorRelic>(),
            RelicOption<RhizomaticAcceleratorRelic>(),
            RelicOption<RhizomaticSubstrateRelic>(),
        ];

    protected override bool IsCurseAllowed(
        RelicModel curse,
        IReadOnlyCollection<RelicModel> positive
    )
    {
        if (curse is ParallelProcessorRelic)
            return !positive.Any(r => r is JietongHammerRelic or RhizomaticStabilizerRelic);

        return true;
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
        [.. base.GenerateInitialOptions(), RelicOption<SealOfInnovationRelic>()];
}
