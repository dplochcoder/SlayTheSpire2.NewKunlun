namespace NewKunlun.NewKunlunCode.Localization;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class RelicLocalizationAttribute(string title, string description, string flavor)
    : Attribute
{
    public string Title { get; } = title;
    public string Description { get; } = description;
    public string Flavor { get; } = flavor;
}
