using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Relics.Ancients.Shuanshuan;

namespace NewKunlun.NewKunlunCode.Ancients;

public class ShuanShuanAncient : NewKunlunAncient
{
    protected override List<List<EventOption>> PositiveOptions =>
        [
            [RelicOption<AncientSheetMusicRelic>(), RelicOption<FourTreasuresOfTheStudyRelic>()],
            [RelicOption<CrimsonHibiscusRelic>(), RelicOption<UnknownSeedRelic>()],
            [RelicOption<FusangAmuletRelic>()],
            [RelicOption<MultiToolKitRelic>()],
        ];

    protected override List<EventOption> CurseOptions =>
        [RelicOption<QiankunBoardRelic>(), RelicOption<VirtualRealityDeviceRelic>()];

    protected override bool IsCurseAllowed(
        RelicModel curse,
        IReadOnlyCollection<RelicModel> positive
    )
    {
        if (curse is QiankunBoardRelic or VirtualRealityDeviceRelic)
            return !positive.Any(r => r is AncientSheetMusicRelic);

        return true;
    }
}
