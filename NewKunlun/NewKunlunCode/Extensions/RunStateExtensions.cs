using MegaCrit.Sts2.Core.Runs;
using NewKunlun.NewKunlunCode.Acts;

namespace NewKunlun.NewKunlunCode.Extensions;

public static class RunStateExtensions
{
    extension(IRunState self)
    {
        public bool VisitsNewKunlun() => self.Acts.Any(a => a is NewKunlunAct);
    }
}
