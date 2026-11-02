using MegaCrit.Sts2.Core.Events;
using NewKunlun.NewKunlunCode.Relics.Ancients.Shuanshuan;

namespace NewKunlun.NewKunlunCode.Ancients;

public class ShuanShuanAncient : NewKunlunAncient
{
    protected override List<List<EventOption>> PositiveOptions =>
        [
            [RelicOption<CrimsonHibiscusRelic>()],
            [RelicOption<FusangAmuletRelic>()],
        ];

    protected override List<EventOption> CurseOptions => [RelicOption<QiankunBoardRelic>()];
}
