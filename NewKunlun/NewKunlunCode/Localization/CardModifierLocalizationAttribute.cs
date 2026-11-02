namespace NewKunlun.NewKunlunCode.Localization;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class CardModifierLocalizationAttribute(
    string title,
    string description,
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
    );
