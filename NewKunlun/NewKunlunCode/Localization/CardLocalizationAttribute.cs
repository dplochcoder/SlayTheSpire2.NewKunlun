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
) : Attribute
{
    public string Title { get; } = title;
    public string Description { get; } = description;
    public string? SelectionScreenPrompt { get; } = selectionScreenPrompt;
    public string? CustomPromptA { get; } = customPromptA;
    public string? CustomPromptB { get; } = customPromptB;
    public string? CustomPromptC { get; } = customPromptC;
    public string? CustomPromptD { get; } = customPromptD;
}
