namespace NewKunlun.NewKunlunCode.Localization;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AfflictionLocalizationAttribute(
    string title,
    string description,
    string? extraCardText = null,
    bool skipValidation = false
) : BaseLocalizationAttribute(title, description, skipValidation: skipValidation)
{
    public string? ExtraCardText { get; } = extraCardText;
}
