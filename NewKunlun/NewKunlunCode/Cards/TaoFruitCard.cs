using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;
using SmartFormat.Core.Extensions;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Tao Fruit",
    description: "{TipContext:taoFruit:Gain {Value} [gold]{Attr}[/gold].|\n}",
    skipValidation: true
)]
public partial class TaoFruitCard()
    : NewKunlunCard(1, CardType.Power, CardRarity.Token, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DynamicVar(nameof(TipContext), 1M),
            new DynamicVar(nameof(Boost), 0M),
            new DynamicVar(nameof(Strength), 0M),
            new DynamicVar(nameof(Speed), 0M),
            new DynamicVar(nameof(Dexterity), 0M),
            new DynamicVar(nameof(Vitality), 0M),
        ];

    public IEnumerable<(DynamicVar stat, Func<IHoverTip> tip)> Stats =>
        [
            (Boost, Tip.Boost),
            (Strength, Tip.Strength),
            (Speed, Tip.Speed),
            (Dexterity, Tip.Dexterity),
            (Vitality, Tip.Vitality),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        Stats.Where(s => TipContext.BaseValue > 0 || s.stat.BaseValue > 0).Select(s => s.tip());

    private static IReadOnlyList<Action<TaoFruitCard>> _generators =
    [
        c => c.Boost.BaseValue += 8,
        c => c.Strength.BaseValue += 2,
        c => c.Speed.BaseValue++,
        c => c.Dexterity.BaseValue += 2,
        c => c.Vitality.BaseValue++,
    ];

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Innate);
}

file class TaoFruitFormatter : IAutoRegisterFormatSpecifier
{
    public string Name
    {
        get => "taoFruit";
        set => throw new InvalidOperationException();
    }

    public bool CanAutoDetect { get; set; }

    public bool TryEvaluateFormat(IFormattingInfo formattingInfo)
    {
        if (
            formattingInfo.Format is not { } format
            || formattingInfo.CurrentValue is not DynamicVar dynamicVar
            || dynamicVar._owner is not TaoFruitCard card
        )
            return false;

        var parts = format.Split('|');
        if (parts.Count != 2)
            return false;
        var elementFormat = parts[0];
        var separatorFormat = parts[1];

        var isTipContext = card.TipContext.BaseValue > 0;
        var printSeparator = false;
        foreach (var stat in card.Stats.Select(s => s.stat))
        {
            if (printSeparator)
                formattingInfo.FormatAsChild(separatorFormat, dynamicVar);

            printSeparator = true;
            if (isTipContext)
                formattingInfo.FormatAsChild(elementFormat, new ElementContext("X", stat.Name));
            else if (stat.BaseValue > 0)
                formattingInfo.FormatAsChild(
                    elementFormat,
                    new ElementContext(stat.BaseValue, stat.Name)
                );
            else
                printSeparator = false;
        }
        return true;
    }

    private sealed class ElementContext(object? value, string attr)
    {
        public object? Value => value;
        public string Attr => attr;
    }
}
