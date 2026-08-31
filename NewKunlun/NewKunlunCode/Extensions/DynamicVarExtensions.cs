using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace NewKunlun.NewKunlunCode.Extensions;

public static class DynamicVarExtensions
{
    public static void UpgradeValueTo(this DynamicVar self, decimal target) =>
        self.UpgradeValueBy(target - self.BaseValue);
}
