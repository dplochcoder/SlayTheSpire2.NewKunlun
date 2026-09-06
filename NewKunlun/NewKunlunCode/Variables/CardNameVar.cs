using System.Reflection;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Localization;
using SmartFormat.Core.Extensions;

namespace NewKunlun.NewKunlunCode.Variables;

public class CardNameVar<TCard>(Func<bool> upgraded)
    : DynamicVar(typeof(TCard).Name[..^4], 0M),
        ICardNameVar
    where TCard : CardModel
{
    private static readonly string Title =
        typeof(TCard).GetCustomAttribute<CardLocalizationAttribute>()?.Title ?? "???";

    public string FormatCardName() =>
        _owner != null && CustomVar.CanCalculate(_owner) && upgraded()
            ? $"[green]{Title}+[/green]"
            : $"[gold]{Title}[/gold]";
}

file interface ICardNameVar
{
    public string FormatCardName();
}

file class CardNameVarFormatter : IAutoRegisterFormatSpecifier
{
    public string Name
    {
        get => "cardName";
        set => throw new InvalidOperationException();
    }

    public bool CanAutoDetect { get; set; }

    public bool TryEvaluateFormat(IFormattingInfo formattingInfo)
    {
        if (formattingInfo.CurrentValue is not ICardNameVar cardNameVar)
            return false;

        formattingInfo.Write(cardNameVar.FormatCardName());
        return true;
    }
}
