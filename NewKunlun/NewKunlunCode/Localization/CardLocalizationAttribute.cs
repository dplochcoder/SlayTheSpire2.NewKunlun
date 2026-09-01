namespace NewKunlun.NewKunlunCode.Localization;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class CardLocalizationAttribute(
    string title,
    string description,
    string? selectionScreenPrompt = null,
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
    public string? SelectionScreenPrompt { get; } = selectionScreenPrompt;
}
