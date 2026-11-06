using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SmartFormat.Core.Extensions;

namespace NewKunlun.NewKunlunCode.Variables;

public class CarveLegacyCostVar() : DynamicVar("CarveLegacyCost", 0M)
{
    public string Format() =>
        _owner == null ? "\nThis card's energy cost is equal to the number of players." : "";
}

file class CarveLegacyCostVarFormatter : IAutoRegisterFormatSpecifier
{
    public string Name
    {
        get => "carveLegacyCost";
        set => throw new InvalidOperationException();
    }

    public bool CanAutoDetect { get; set; }

    public bool TryEvaluateFormat(IFormattingInfo formattingInfo)
    {
        if (formattingInfo.CurrentValue is not CarveLegacyCostVar carveLegacyCostVar)
            return false;

        formattingInfo.Write(carveLegacyCostVar.Format());
        return true;
    }
}
