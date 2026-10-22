using MegaCrit.Sts2.Core.Events;
using NewKunlun.NewKunlunCode.Relics;

namespace NewKunlun.NewKunlunCode.Ancients;

public class AbacusAncient : NewKunlunAncient
{
    protected override List<List<EventOption>> PositiveOptions =>
        [
            [RelicOption<BalancedBladeRelic>(), RelicOption<SwiftCloakRelic>()],
            [RelicOption<FirewallRelic>()],
            [RelicOption<JietongHammerRelic>()],
            [RelicOption<MappingChipRelic>(), RelicOption<MetalDetectorRelic>()],
            [RelicOption<MysticNymphRelic>(), RelicOption<ZenGardenRelic>()],
            [RelicOption<TaoFruitRelic>()],
            [RelicOption<ScrapRelic>()],
            [RelicOption<SealOfInnovationRelic>()],
        ];

    protected override List<EventOption> CurseOptions =>
        [
            RelicOption<CentrifugeRelic>(),
            RelicOption<MassSpectrometerRelic>(),
            RelicOption<UnstableSubstrateRelic>(),
        ];
}
