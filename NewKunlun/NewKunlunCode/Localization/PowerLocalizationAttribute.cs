namespace NewKunlun.NewKunlunCode.Localization;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class PowerLocalizationAttribute(
    string title,
    string description,
    string? smartDescription = null,
    string? remoteDescription = null,
    string? selectionScreenPrompt = null,
    string? customPromptA = null,
    string? customPromptB = null,
    string? customPromptC = null,
    string? customPromptD = null,
    bool skipValidation = false
)
    : BaseLocalizationAttribute(
        title,
        description,
        customPromptA: customPromptA,
        customPromptB: customPromptB,
        customPromptC: customPromptC,
        customPromptD: customPromptD,
        skipValidation: skipValidation
    )
{
    public string SmartDescription { get; } = smartDescription ?? description;
    public string? RemoteDescription { get; } = remoteDescription;
    public string? SelectionScreenPrompt { get; } = selectionScreenPrompt;
}
