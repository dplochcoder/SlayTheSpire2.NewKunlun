using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "3D Printer",
    description: "Spend {Cost} gold. Choose a card in your hand. Add a copy of it into your hand. Permanently increase cost by {CostIncrement:diff()} gold.",
    selectionScreenPrompt: "Choose a card to 3D Print into your hand."
)]
public partial class Printer3DCard()
    : NewKunlunCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        this.BuildKeywords(CardKeyword.Exhaust).IfUpgraded(CardKeyword.Retain);

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new GoldVar(nameof(Cost), 5), new GoldVar(nameof(CostIncrement), 5)];

    protected override void OnUpgrade() => CostIncrement.UpgradeValueTo(3M);

    [SavedProperty]
    public int CostIncrease
    {
        get;
        set
        {
            AssertMutable();
            field = value;
            UpdateValues();
        }
    }

    protected override void AfterDowngraded() => UpdateValues();

    private void UpdateValues() => Cost.BaseValue = 5 + CostIncrease;

    protected override bool IsPlayable => Owner.Gold >= Cost.BaseValue;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.Gold < Cost.BaseValue)
            return;

        var cards = await CardSelectCmd.FromCombatPile(
            choiceContext,
            PileType.Hand.GetPile(Owner),
            Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 1)
        );

        await PlayerCmd.LoseGold(Cost.BaseValue, Owner, GoldLossType.Spent);
        foreach (var card in cards)
            await CardPileCmd.Add(card.CreateClone(), PileType.Hand.GetPile(Owner), clonedBy: this);

        this.Permanently(c => c.CostIncrease += (int)CostIncrement.BaseValue);
    }
}
