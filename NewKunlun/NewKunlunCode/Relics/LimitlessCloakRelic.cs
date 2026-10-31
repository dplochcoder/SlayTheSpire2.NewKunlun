using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Limitless Cloak",
    description: "Whenever you play a card, draw a card.\nYou cannot play more than {CardLimit} cards per turn.",
    flavor: ""
)]
public partial class LimitlessCloakRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new(nameof(CardLimit), 10)];

    [SavedProperty]
    public int CardsPlayedThisTurn
    {
        get;
        set
        {
            AssertMutable();
            field = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override bool ShowCounter => CombatManager.Instance.IsInProgress;

    public override int DisplayAmount => CardsPlayedThisTurn;

    private bool ShouldPreventCardPlay => CardsPlayedThisTurn >= CardLimit.IntValue;

    public override bool ShouldPlay(CardModel card, AutoPlayType _) =>
        Owner != card.Owner || !ShouldPreventCardPlay;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner)
            return;

        Flash();
        ++CardsPlayedThisTurn;
        await CardPileCmd.Draw(choiceContext, Owner);
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom)
            CardsPlayedThisTurn = 0;
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        CardsPlayedThisTurn = 0;
        return Task.CompletedTask;
    }

    public override Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (side == Owner.Creature.Side)
            CardsPlayedThisTurn = 0;
        return Task.CompletedTask;
    }
}
