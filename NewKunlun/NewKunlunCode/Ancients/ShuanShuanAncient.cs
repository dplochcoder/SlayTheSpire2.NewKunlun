using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Relics.Ancients.Shuanshuan;

namespace NewKunlun.NewKunlunCode.Ancients;

public class ShuanShuanAncient : NewKunlunAncient
{
    protected override List<List<EventOption>> PositiveOptions =>
        [
            [RelicOption<AncientSheetMusicRelic>(), RelicOption<GmFertilizerRelic>()],
            [
                RelicOption<CrimsonHibiscusRelic>(),
                RelicOption<TiandaoAcademyPeriodicalRelic>(),
                RelicOption<UnknownSeedRelic>(),
            ],
            [RelicOption<FourTreasuresOfTheStudyRelic>(), RelicOption<MultiToolKitRelic>()],
            [RelicOption<FusangAmuletRelic>(), RelicOption<RedGuifangClayRelic>()],
            [RelicOption<ReadyToEatRationsRelic>()],
        ];

    protected override List<EventOption> CurseOptions =>
        [
            RelicOption<AntiqueVinylRecordRelic>(),
            RelicOption<QiankunBoardRelic>(),
            RelicOption<VirtualRealityDeviceRelic>(),
        ];

    protected override bool IsCurseAllowed(
        RelicModel curse,
        IReadOnlyCollection<RelicModel> positive
    )
    {
        if (curse is AntiqueVinylRecordRelic or QiankunBoardRelic or VirtualRealityDeviceRelic)
            return !positive.Any(r => r is AncientSheetMusicRelic or GmFertilizerRelic);

        return true;
    }
}
