using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Ancients;

namespace NewKunlun.NewKunlunCode.Acts;

public abstract class NewKunlunAct(int actNumber) : CustomActModel(actNumber)
{
    public override IEnumerable<AncientEventModel> AllAncients =>
        Index switch
        {
            0 => NewKunlunAncient.ActOneAncients,
            1 => NewKunlunAncient.ActTwoAncients,
            2 => NewKunlunAncient.ActThreeAncients,
            _ => [],
        };
}
