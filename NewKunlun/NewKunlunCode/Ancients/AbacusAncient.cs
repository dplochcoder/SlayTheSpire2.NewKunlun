using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using NewKunlun.NewKunlunCode.Relics.Ancients.Abacus;
using CentrifugeRelic = NewKunlun.NewKunlunCode.Relics.Ancients.Abacus.CentrifugeRelic;
using DefenseMatrixRelic = NewKunlun.NewKunlunCode.Relics.Ancients.Abacus.DefenseMatrixRelic;
using FirewallRelic = NewKunlun.NewKunlunCode.Relics.Ancients.Abacus.FirewallRelic;
using FusangRootRelic = NewKunlun.NewKunlunCode.Relics.Ancients.Abacus.FusangRootRelic;
using JietongHammerRelic = NewKunlun.NewKunlunCode.Relics.Ancients.Abacus.JietongHammerRelic;
using MapDataChipRelic = NewKunlun.NewKunlunCode.Relics.Ancients.Abacus.MapDataChipRelic;
using MetalDetectorRelic = NewKunlun.NewKunlunCode.Relics.Ancients.Abacus.MetalDetectorRelic;
using QiBladeRelic = NewKunlun.NewKunlunCode.Relics.Ancients.Abacus.QiBladeRelic;
using RhizomaticSubstrateRelic = NewKunlun.NewKunlunCode.Relics.Ancients.Abacus.RhizomaticSubstrateRelic;
using ScrapRelic = NewKunlun.NewKunlunCode.Relics.Ancients.Abacus.ScrapRelic;
using TaoFruitRelic = NewKunlun.NewKunlunCode.Relics.Ancients.Abacus.TaoFruitRelic;

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
