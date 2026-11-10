using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Acts;

[ActLocalization(title: "Empyrean District")]
public class EmpyreanDistrictAct() : NewKunlunAct(actNumber: 3)
{
    // TODO
    public override IEnumerable<EncounterModel> GenerateAllEncounters() => [];

    public override IEnumerable<EventModel> AllEvents => [];
    protected override string CustomMapTopBgPath => ModelDb.Act<Overgrowth>().MapTopBgPath;
    protected override string CustomMapMidBgPath => ModelDb.Act<Overgrowth>().MapMidBgPath;
    protected override string CustomMapBotBgPath => ModelDb.Act<Overgrowth>().MapBotBgPath;
    protected override string CustomRestSiteBackgroundPath =>
        ModelDb.Act<Glory>().RestSiteBackgroundPath;
}
