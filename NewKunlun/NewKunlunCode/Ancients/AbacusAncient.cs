using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Relics.Ancients.Abacus;

namespace NewKunlun.NewKunlunCode.Ancients;

public class AbacusAncient : NewKunlunAncient
{
    protected override List<List<EventOption>> PositiveOptions =>
        [
            [RelicOption<AzureGlassRelic>(), RelicOption<BackupPlanRelic>()],
            [RelicOption<BalancedBladeRelic>(), RelicOption<SwiftCloakRelic>()],
            [
                RelicOption<BunsenBurnerRelic>(),
                RelicOption<JieHammerRelic>(),
                RelicOption<RhizomaticStabilizerRelic>(),
            ],
            [
                RelicOption<ComputingUnitRelic>(),
                RelicOption<RhizomaticCoreRelic>(),
                RelicOption<ScrapRelic>(),
            ],
            [RelicOption<DefenseMatrixRelic>(), RelicOption<FirewallRelic>()],
            [
                RelicOption<EmergencyKitRelic>(),
                RelicOption<MassSpectrometerRelic>(),
                RelicOption<QiBladeRelic>(),
            ],
            [RelicOption<MapDataChipRelic>(), RelicOption<MetalDetectorRelic>()],
            [
                RelicOption<MysticNymphRelic>(),
                RelicOption<ZenGardenRelic>(),
                RelicOption<RhizomaticGeneratorRelic>(),
            ],
        ];

    protected override List<EventOption> CurseOptions =>
        [
            RelicOption<BloodyCrimsonHibiscus>(),
            RelicOption<CentrifugeRelic>(),
            RelicOption<FangshiStatueRelic>(),
            RelicOption<FusangRootRelic>(),
            RelicOption<GrowthSerumRelic>(),
            RelicOption<LockedChestRelic>(),
            RelicOption<ParallelProcessorRelic>(),
            RelicOption<ReconnaissanceRelic>(),
            RelicOption<RhizomaticAcceleratorRelic>(),
            RelicOption<RhizomaticSubstrateRelic>(),
        ];

    protected override bool IsCurseAllowed(
        RelicModel curse,
        IReadOnlyCollection<RelicModel> positive
    ) =>
        curse switch
        {
            CentrifugeRelic => !positive.Any(r => r is BunsenBurnerRelic),
            ParallelProcessorRelic => !positive.Any(r =>
                r is JieHammerRelic or RhizomaticStabilizerRelic
            ),
            ReconnaissanceRelic => !positive.Any(r => r is DefenseMatrixRelic or FirewallRelic),
            _ => true,
        };

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
        [.. base.GenerateInitialOptions(), RelicOption<SealOfInnovationRelic>()];
}
