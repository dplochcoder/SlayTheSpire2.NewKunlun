namespace NewKunlun.NewKunlunCode.Localization;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class PowerLocalizationAttribute(
    string title,
    string description,
    string smartDescription
) : Attribute
{
    public string Title { get; } = title;
    public string Description { get; } = description;
    public string SmartDescription { get; } = smartDescription;
}
