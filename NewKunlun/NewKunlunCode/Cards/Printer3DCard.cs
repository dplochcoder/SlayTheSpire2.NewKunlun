using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "3D Printer",
    description: "Spend {Gold:diff()} gold. Choose a card in your hand. Add a copy of it into your hand. Cannot print itself."
)]
public partial class Printer3DCard()
    : NewKunlunCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        this.BuildKeywords(CardKeyword.Exhaust).IfUpgraded(CardKeyword.Retain);

    protected override IEnumerable<DynamicVar> CanonicalVars => [new GoldVar(10)];

    protected override bool IsPlayable => Owner.Gold >= Gold.BaseValue;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.Gold < Gold.BaseValue)
            return;

        var cards = await CardSelectCmd.FromCombatPile(
            choiceContext,
            PileType.Hand.GetPile(Owner),
            Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 1),
            card => card is not Printer3DCard
        );

        await PlayerCmd.LoseGold(Gold.BaseValue, Owner, GoldLossType.Spent);
        foreach (var card in cards)
            await CardPileCmd.Add(card.CreateClone(), PileType.Hand.GetPile(Owner), clonedBy: this);
    }
}
