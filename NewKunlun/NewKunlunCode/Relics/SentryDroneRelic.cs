using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Acts;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(YiRelicPool))]
[RelicLocalization(
    title: "Sentry Drone",
    description: "At the end of your turn, if you played no attacks this turn, deal {Damage} damage to all enemies.",
    flavor: ""
)]
public partial class SentryDroneRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override bool IsAllowed(IRunState runState) => runState.Act is NewKunlunAct;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(9, ValueProp.Unpowered)];

    public override Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (
            side == Owner.Creature.Side
            && !Owner.PlayedAnyCardThisTurn(c => c.Type == CardType.Attack)
        )
            Status = RelicStatus.Active;

        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == Owner && cardPlay.Card.Type == CardType.Attack)
            Status = RelicStatus.Normal;

        return Task.CompletedTask;
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (
            side != Owner.Creature.Side
            || Owner.Creature.CombatState == null
            || Owner.PlayedAnyCardThisTurn(c => c.Type == CardType.Attack)
        )
            return;

        Flash();

        // TODO: Pet
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await DamageCmd
            .Attack(Damage.BaseValue)
            .Unpowered()
            .TargetingAllOpponents(Owner.Creature.CombatState!)
            .WithSweepingBeamVfx(Owner)
            .Execute(choiceContext);
    }
}
