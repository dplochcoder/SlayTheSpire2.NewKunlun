namespace NewKunlun.NewKunlunCode.Localization;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class RelicLocalizationAttribute(
    string title,
    string description,
    string flavor,
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
    public string Flavor { get; } = flavor;
}
