namespace NewKunlun.NewKunlunCode.Localization;

[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class KeywordLocalizationAttribute(string title, string description) : Attribute
{
    public string Title { get; } = title;
    public string Description { get; } = description;
}
