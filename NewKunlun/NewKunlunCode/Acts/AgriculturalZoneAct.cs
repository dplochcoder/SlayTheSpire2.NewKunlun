using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using NewKunlun.NewKunlunCode.Ancients;

namespace NewKunlun.NewKunlunCode.Acts;

public class AgriculturalZoneAct() : CustomActModel(actNumber: 1)
{
    public override IEnumerable<EncounterModel> GenerateAllEncounters()
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<AncientEventModel> AllAncients => NewKunlunAncient.ActOneAncients;

    public override IEnumerable<EventModel> AllEvents { get; }
    protected override string CustomMapTopBgPath => ModelDb.Act<Underdocks>().MapTopBgPath;
    protected override string CustomMapMidBgPath => ModelDb.Act<Underdocks>().MapMidBgPath;
    protected override string CustomMapBotBgPath => ModelDb.Act<Underdocks>().MapBotBgPath;
    protected override string CustomRestSiteBackgroundPath =>
        ModelDb.Act<Underdocks>().RestSiteBackgroundPath;
}
