namespace NewKunlun.NewKunlunCode.Localization;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class EnchantmentLocalizationAttribute(
    string title,
    string description,
    string? extraCardText = null,
    bool skipValidation = false
) : BaseLocalizationAttribute(title, description, skipValidation: skipValidation)
{
    public string? ExtraCardText { get; } = extraCardText;
}
