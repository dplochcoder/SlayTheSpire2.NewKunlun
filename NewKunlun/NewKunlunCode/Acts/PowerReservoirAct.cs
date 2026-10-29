using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using NewKunlun.NewKunlunCode.Ancients;

namespace NewKunlun.NewKunlunCode.Acts;

public class PowerReservoirAct() : NewKunlunAct(actNumber: 1)
{
    public override IEnumerable<EncounterModel> GenerateAllEncounters()
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<AncientEventModel> AllAncients => NewKunlunAncient.ActOneAncients;

    public override IEnumerable<EventModel> AllEvents => [];
    protected override string CustomMapTopBgPath => ModelDb.Act<Overgrowth>().MapTopBgPath;
    protected override string CustomMapMidBgPath => ModelDb.Act<Overgrowth>().MapMidBgPath;
    protected override string CustomMapBotBgPath => ModelDb.Act<Overgrowth>().MapBotBgPath;
    protected override string CustomRestSiteBackgroundPath =>
        ModelDb.Act<Glory>().RestSiteBackgroundPath;
}
