namespace NewKunlun.NewKunlunCode.Localization;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class RelicLocalizationAttribute(
    string title,
    string description,
    string flavor,
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
    public string Flavor { get; } = flavor;
}
