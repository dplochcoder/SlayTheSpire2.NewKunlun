namespace NewKunlun.NewKunlunCode.Localization;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class CardLocalizationAttribute(
    string title,
    string description,
    string? selectionScreenPrompt = null
) : Attribute
{
    public string Title { get; } = title;
    public string Description { get; } = description;
    public string? SelectionScreenPrompt { get; } = selectionScreenPrompt;
}
