using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Helpers;

namespace NewKunlun.NewKunlunCode.Extensions;

public static class AscensionLevelExtensions
{
    extension(AscensionLevel self)
    {
        public decimal Get(decimal ascended, decimal basic) =>
            AscensionHelper.GetValueIfAscension(self, ascended, basic);

        public float Get(float ascended, float basic) =>
            AscensionHelper.GetValueIfAscension(self, ascended, basic);

        public int Get(int ascended, int basic) =>
            AscensionHelper.GetValueIfAscension(self, ascended, basic);
    }
}
