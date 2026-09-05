using System.Reflection;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Cards;
using NewKunlun.NewKunlunCode.Localization;
using SmartFormat;
using SmartFormat.Core.Extensions;

namespace NewKunlun.NewKunlunCode.Variables;

public class CardNameVar<TOwner, TCard>(string name, Func<TOwner, bool> upgraded)
    : DynamicVar(name, 0M),
        ICardNameVar
    where TOwner : AbstractModel
    where TCard : CardModel
{
    private static readonly string Title =
        typeof(TCard).GetCustomAttribute<CardLocalizationAttribute>()?.Title ?? "???";
    private new TOwner? _owner;

    public override void SetOwner(AbstractModel owner)
    {
        base.SetOwner(owner);
        _owner = (TOwner)owner;
    }

    public string FormatCardName() =>
        _owner != null && upgraded(_owner) ? $"[green]{Title}+[/green]" : $"[gold]{Title}[/gold]";
}

public class TalismanDashVar<TOwner>(Func<TOwner, bool> upgraded)
    : CardNameVar<TOwner, TalismanDashCard>("TalismanDash", upgraded)
    where TOwner : AbstractModel { }

public class TalismanDetonateVar<TOwner>(Func<TOwner, bool> upgraded)
    : CardNameVar<TOwner, TalismanDetonateCard>("TalismanDetonate", upgraded)
    where TOwner : AbstractModel { }

file interface ICardNameVar
{
    public string FormatCardName();

    static ICardNameVar() => Smart.Default.AddExtensions(new CardNameVarFormatter());
}

[CustomFormatter]
file class CardNameVarFormatter : IFormatter
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
