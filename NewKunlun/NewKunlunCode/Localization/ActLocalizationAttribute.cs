namespace NewKunlun.NewKunlunCode.Localization;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ActLocalizationAttribute(string title, bool skipValidation = false) : Attribute
{
    public string Title { get; } = title;
    public bool SkipValidation { get; } = skipValidation;
}
