using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Keywords;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Modifiers;

[CardModifierLocalization(
    title: "Quantum",
    description: "!!!DESCRIPTION!!!" // Displayed as keyword.
)]
public class QuantumModifier : NewKunlunModifier
{
    public override ModifierStackType StackType => ModifierStackType.Single;

    public bool AddedToHand
    {
        get;
        private set
        {
            AssertMutable();
            field = value;
        }
    }

    public bool IsBroken
    {
        get;
        private set
        {
            AssertMutable();
            field = value;
        }
    }

    public override Task AfterCardChangedPiles(
        CardModel card,
        PileType oldPileType,
        AbstractModel? clonedBy
    )
    {
        if (card == Owner && oldPileType != PileType.Hand && card.Pile?.Type == PileType.Hand)
            AddedToHand = true;

        return Task.CompletedTask;
    }

    public override bool TryModifyKeywordsInCombat(CardModel card, ISet<CardKeyword> keywords)
    {
        if (card != Owner)
            return false;

        keywords.Add(ExtraCardKeywords.Quantum);
        if (IsBroken)
            keywords.Add(CardKeyword.Unplayable);
        return true;
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (Owner == null || cardPlay.Card.Owner != Owner.Owner)
            return Task.CompletedTask;

        if (cardPlay.Card == Owner)
            RemoveModifier(cardPlay.Card, this);
        else if (AddedToHand && !IsBroken)
            IsBroken = true;

        return Task.CompletedTask;
    }
}
