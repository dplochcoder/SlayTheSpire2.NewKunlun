namespace NewKunlun.NewKunlunCode.Localization;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public abstract class BaseLocalizationAttribute(
    string title,
    string description,
    string? customPromptA = null,
    string? customPromptB = null,
    string? customPromptC = null,
    string? customPromptD = null,
    bool skipValidation = false
) : Attribute
{
    public string Title { get; } = title;
    public string Description { get; } = description;
    public string? CustomPromptA { get; } = customPromptA;
    public string? CustomPromptB { get; } = customPromptB;
    public string? CustomPromptC { get; } = customPromptC;
    public string? CustomPromptD { get; } = customPromptD;
    public bool SkipValidation { get; } = skipValidation;
}
