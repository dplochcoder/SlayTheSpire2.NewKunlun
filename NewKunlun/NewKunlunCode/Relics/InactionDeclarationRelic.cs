using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Inaction Declaration",
    description: "Once per combat, if you end your turn without playing any cards, gain {Block} [gold]Block[/gold] and keep your energy until next turn.",
    flavor: ""
)]
public partial class InactionDeclarationRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(32, ValueProp.Unpowered)];

    private bool UsedThisCombat
    {
        get => field;
        set
        {
            AssertMutable();
            field = value;
        }
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        Status = RelicStatus.Normal;
        UsedThisCombat = false;
        return Task.CompletedTask;
    }

    public override Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (side == Owner.Creature.Side && !UsedThisCombat)
            Status = RelicStatus.Active;

        return Task.CompletedTask;
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (
            CombatManager.Instance.IsInProgress
            && !UsedThisCombat
            && !cardPlay.IsAutoPlay
            && cardPlay.Card.Owner == Owner
        )
            Status = RelicStatus.Normal;

        return Task.CompletedTask;
    }

    // Copied from PaelsEye
    private bool AnyCardsPlayedThisTurn()
    {
        var playerCombatState = Owner.PlayerCombatState;
        return playerCombatState?.TurnNumber is 1 && Owner.Relics.Any(r => r is WhisperingEarring)
            || CombatManager.Instance.History.CardPlaysFinished.Any(e =>
                e.Actor == Owner.Creature
                && e.HappenedThisTurn(Owner.Creature.CombatState)
                && !e.CardPlay.IsAutoPlay
            );
    }

    public override async Task BeforeSideTurnEndEarly(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (side != Owner.Creature.Side || UsedThisCombat || AnyCardsPlayedThisTurn())
            return;

        Flash();
        UsedThisCombat = true;
        await CreatureCmd.GainBlock(Owner.Creature, Block, null);
        await PowerCmd.Apply<KeepEnergyThisTurnPower>(
            choiceContext,
            Owner.Creature,
            1,
            Owner.Creature,
            null
        );
    }
}
