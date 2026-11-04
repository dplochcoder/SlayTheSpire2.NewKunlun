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

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Lear;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Limitless Cloak",
    description: "Whenever you play a card, draw a card.\nYou cannot play more than {CardLimit} cards per turn.",
    flavor: ""
)]
public partial class LimitlessCloakRelic : NewKunlunRelic
{
    private const int CardsPerTurn = 8;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new(nameof(CardLimit), CardsPerTurn)];

    [SavedProperty]
    public int CardsRemainingThisTurn
    {
        get;
        set
        {
            AssertMutable();
            field = value;
            InvokeDisplayAmountChanged();
        }
    } = CardsPerTurn;

    public override bool ShowCounter => CombatManager.Instance.IsInProgress;

    public override int DisplayAmount => CardsRemainingThisTurn;

    private bool ShouldPreventCardPlay => CardsRemainingThisTurn <= 0;

    public override bool ShouldPlay(CardModel card, AutoPlayType _) =>
        Owner != card.Owner || !ShouldPreventCardPlay;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner)
            return;

        Flash();
        --CardsRemainingThisTurn;
        await CardPileCmd.Draw(choiceContext, Owner);
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom)
            CardsRemainingThisTurn = CardLimit.IntValue;
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        CardsRemainingThisTurn = CardLimit.IntValue;
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
            CardsRemainingThisTurn = CardsPerTurn;
        return Task.CompletedTask;
    }
}
