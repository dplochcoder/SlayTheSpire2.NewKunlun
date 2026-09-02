using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Medicine Pipe",
    description: "Heal {Heal:diff()} hp. Heal all [gold]Internal Damage[/gold]. After {RemainingUses:diff()}, this card is removed from your deck."
)]
public partial class MedicinePipeCard()
    : NewKunlunCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DynamicVar(nameof(Heal), 12M),
            new DynamicVar(nameof(TotalUses), 3M),
            new DynamicVar(nameof(RemainingUses), 3M),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tips.InternalDamage()];

    [SavedProperty]
    public int TimesUsed
    {
        get => field;
        set
        {
            AssertMutable();
            field = value;
            UpdateValues();
        }
    }

    private void UpdateValues() =>
        RemainingUses.BaseValue = Math.Max(0, TotalUses.BaseValue - TimesUsed);

    protected override void OnUpgrade()
    {
        Heal.UpgradeValueTo(16M);
        TotalUses.UpgradeValueTo(4M);
        RemainingUses.UpgradeValueBy(1M);
    }

    protected override void AfterDowngraded() => UpdateValues();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Heal(Owner.Creature, Heal.BaseValue);
        await InternalDamageCmd.Heal(
            choiceContext,
            Owner.Creature,
            new InternalDamageHealVar(decimal.MaxValue),
            Owner.Creature,
            this
        );

        var deckVersion = this.Permanently(c => ++c.TimesUsed);
        if (deckVersion is { RemainingUses.BaseValue: 0 })
            await CardPileCmd.RemoveFromDeck(deckVersion);
    }
}
