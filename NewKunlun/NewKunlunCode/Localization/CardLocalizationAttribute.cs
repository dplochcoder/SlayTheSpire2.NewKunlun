namespace NewKunlun.NewKunlunCode.Localization;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class CardLocalizationAttribute(string title, string description) : Attribute
{
    public string Title { get; } = title;
    public string Description { get; } = description;
}
