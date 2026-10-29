using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Relics;

namespace NewKunlun.NewKunlunCode.Ancients;

public class AbacusAncient : NewKunlunAncient
{
    protected override List<List<EventOption>> PositiveOptions =>
        [
            [RelicOption<BalancedBladeRelic>(), RelicOption<SwiftCloakRelic>()],
            [RelicOption<CrimsonHibiscusRelic>()],
            [RelicOption<DefenseMatrixRelic>(), RelicOption<FirewallRelic>()],
            [RelicOption<JietongHammerRelic>(), RelicOption<RhizomaticStabilizerRelic>()],
            [
                RelicOption<MappingChipRelic>(),
                RelicOption<MetalDetectorRelic>(),
                RelicOption<RecyclerRelic>(),
            ],
            [RelicOption<MassSpectrometerRelic>()],
            [RelicOption<MysticNymphRelic>(), RelicOption<ZenGardenRelic>()],
            [RelicOption<RhizomaticAcceleratorRelic>(), RelicOption<SealOfInnovationRelic>()],
            [RelicOption<ScrapRelic>()],
            [RelicOption<TaoFruitRelic>()],
        ];

    protected override List<EventOption> CurseOptions =>
        [
            RelicOption<CentrifugeRelic>(),
            RelicOption<ComputingUnitRelic>(),
            RelicOption<FangshiStatueRelic>(),
            RelicOption<FusangRootRelic>(),
            RelicOption<LockedChestRelic>(),
            RelicOption<ParallelProcessorRelic>(),
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
}
