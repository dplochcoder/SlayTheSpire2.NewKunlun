namespace NewKunlun.NewKunlunCode.Localization;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class EnchantmentLocalizationAttribute(
    string title,
    string description,
    string? extraCardText = null,
    string? customPromptA = null,
    string? customPromptB = null,
    string? customPromptC = null,
    string? customPromptD = null,
    bool skipValidation = false
)
    : BaseLocalizationAttribute(
        title: title,
        description: description,
        customPromptA: customPromptA,
        customPromptB: customPromptB,
        customPromptC: customPromptC,
        customPromptD: customPromptD,
        skipValidation: skipValidation
    )
{
    public string? ExtraCardText { get; } = extraCardText;
}
