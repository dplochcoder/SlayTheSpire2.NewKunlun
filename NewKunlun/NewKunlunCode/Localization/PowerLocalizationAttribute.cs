namespace NewKunlun.NewKunlunCode.Localization;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class PowerLocalizationAttribute(
    string title,
    string description,
    string smartDescription,
    string? remoteDescription = null,
    string? customPromptA = null,
    string? customPromptB = null,
    string? customPromptC = null,
    string? customPromptD = null
)
    : BaseLocalizationAttribute(
        title,
        description,
        customPromptA,
        customPromptB,
        customPromptC,
        customPromptD
    )
{
    public string SmartDescription { get; } = smartDescription;
    public string? RemoteDescription { get; } = remoteDescription;
}
