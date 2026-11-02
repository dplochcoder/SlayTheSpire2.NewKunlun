using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace NewKunlun.NewKunlunCode.Relics;

public abstract class FreeCardsThisCombatRelic : NewKunlunRelic
{
    protected abstract int MaxUsesPerCombat { get; }

    protected abstract bool ShouldBeFree(CardModel card);

    [SavedProperty]
    public int TimesUsedThisCombat
    {
        get;
        set
        {
            AssertMutable();
            field = value;
            Status =
                CombatManager.Instance.IsInProgress && TimesUsedThisCombat < MaxUsesPerCombat
                    ? RelicStatus.Active
                    : RelicStatus.Normal;
            InvokeDisplayAmountChanged();
        }
    }

    public override bool ShowCounter => CombatManager.Instance.IsInProgress;

    public override int DisplayAmount => TimesUsedThisCombat;

    public sealed override Task AfterRoomEntered(AbstractRoom room)
    {
        TimesUsedThisCombat = 0;
        return Task.CompletedTask;
    }

    public override bool TryModifyEnergyCostInCombatLate(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost
    )
    {
        modifiedCost = originalCost;
        if (card.Owner != Owner || !ShouldBeFree(card) || TimesUsedThisCombat >= MaxUsesPerCombat)
            return false;

        var pile = card.Pile?.Type;
        if (pile is not (PileType.Hand or PileType.Play))
            return false;

        modifiedCost = 0;
        return true;
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (
            cardPlay.Card.Owner != Owner
            || !ShouldBeFree(cardPlay.Card)
            || TimesUsedThisCombat >= MaxUsesPerCombat
        )
            return Task.CompletedTask;

        var pile = cardPlay.Card.Pile?.Type;
        if (pile is not (PileType.Hand or PileType.Play))
            return Task.CompletedTask;

        --TimesUsedThisCombat;
        return Task.CompletedTask;
    }
}
