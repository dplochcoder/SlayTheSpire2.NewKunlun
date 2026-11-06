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
            [
                RelicOption<FusangAmuletRelic>(),
                RelicOption<RedGuifangClayRelic>(),
                RelicOption<SwordOfJieRelic>(),
            ],
            [RelicOption<ReadyToEatRationsRelic>(), RelicOption(PortraitRelic.ForPlayer(Owner!))],
        ];

    protected override List<EventOption> CurseOptions =>
        [
            RelicOption<AntiqueVinylRecordRelic>(),
            RelicOption<KunlunImmortalPortraitRelic>(),
            RelicOption<PenglaiRecipeCollectionRelic>(),
            RelicOption<QiankunBoardRelic>(),
            RelicOption<TheLegendOfThePorkyHeroesRelic>(),
            RelicOption<VirtualRealityDeviceRelic>(),
        ];

    protected override bool IsCurseAllowed(
        RelicModel curse,
        IReadOnlyCollection<RelicModel> positive
    ) =>
        curse switch
        {
            AntiqueVinylRecordRelic or QiankunBoardRelic or VirtualRealityDeviceRelic =>
                !positive.Any(r => r is AncientSheetMusicRelic or GmFertilizerRelic),
            KunlunImmortalPortraitRelic => !positive.Any(r =>
                r is CrimsonHibiscusRelic or TiandaoAcademyPeriodicalRelic or UnknownSeedRelic
            ),
            _ => true,
        };
}
