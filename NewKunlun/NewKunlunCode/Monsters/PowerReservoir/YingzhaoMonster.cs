using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Cards.Statuses;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Monsters.Intents;
using NewKunlun.NewKunlunCode.Powers;

namespace NewKunlun.NewKunlunCode.Monsters.PowerReservoir;

[Monster]
public class YingzhaoMonster : YingzhaoMonsterBase
{
    public enum Phase
    {
        One,
        Two,
    }

    private static int SwingDamage => AscensionLevel.DeadlyEnemies.Get(10, 9);

    private static int DoubleSwingDamage => AscensionLevel.DeadlyEnemies.Get(8, 7);

    private static int HeavySwingDamage(Phase phase) => phase == Phase.One ? 17 : 20;

    private static BlockVar BlockTurnBlock => new(10, ValueProp.Move);

    private static int InitialOvercharge => AscensionLevel.DeadlyEnemies.Get(3, 2);

    private static int LoopOvercharge(Phase phase) =>
        phase == Phase.One ? 2 : AscensionLevel.DeadlyEnemies.Get(3, 2);

    private static int Phase2Dexterity => 2;

    private static int Phase2Overcharge => AscensionLevel.DeadlyEnemies.Get(7, 5);

    private static int Phase2Regen => AscensionLevel.ToughEnemies.Get(10, 9);

    public override int MaxInitialHp => AscensionLevel.ToughEnemies.Get(315, 300);

    private static int Phase2Threshold => AscensionLevel.ToughEnemies.Get(150, 140);

    public override Task AfterAddedToRoom() =>
        PowerCmd.Apply<YingzhaoPhase2>(
            new ThrowingPlayerChoiceContext(),
            Creature,
            Phase2Threshold,
            Creature,
            null
        );

    protected override IEnumerable<AbstractIntent> InitialPowerupIntent => [new BuffIntent()];

    [InitialMove]
    protected override async Task<MoveState> InitialPowerup(IReadOnlyList<Creature> targets)
    {
        await PowerCmd.Apply<OverchargedPower>(
            new ThrowingPlayerChoiceContext(),
            Creature,
            InitialOvercharge,
            Creature,
            null
        );
        return InitialPowerupMove;
    }

    protected override IEnumerable<AbstractIntent> ForwardSwingIntent =>
        [new SingleAttackIntent(SwingDamage), new DebuffIntent()];

    [Move]
    protected override async Task<MoveState> ForwardSwing(IReadOnlyList<Creature> targets)
    {
        await DamageCmd
            .Attack(SwingDamage)
            .FromMonster(this)
            .WithHeavySlashVfx()
            .TargetingAllOpponents(CombatState)
            .Execute(null);
        await PowerCmd.Apply<FrailPower>(
            new ThrowingPlayerChoiceContext(),
            targets,
            1,
            Creature,
            null
        );
        return BackwardSwingMove;
    }

    protected override IEnumerable<AbstractIntent> BackwardSwingIntent =>
        [new SingleAttackIntent(SwingDamage), new DebuffIntent(), new StatusIntent(1)];

    [Move]
    protected override async Task<MoveState> BackwardSwing(IReadOnlyList<Creature> targets)
    {
        await DamageCmd
            .Attack(SwingDamage)
            .FromMonster(this)
            .WithHeavySlashVfx()
            .TargetingAllOpponents(CombatState)
            .Execute(null);
        await PowerCmd.Apply<WeakPower>(
            new ThrowingPlayerChoiceContext(),
            targets,
            1,
            Creature,
            null
        );
        await GiveStatusCards<StaggeredCard>(targets, [new(PileType.Draw, CardPilePosition.Top)]);
        return BlockTurnMove(Phase.One);
    }

    protected override IEnumerable<AbstractIntent> BlockTurnIntent(Phase phase) =>
        [new DefendIntent(), new BuffIntent()];

    [Move]
    protected override async Task<MoveState> BlockTurn(Phase phase, IReadOnlyList<Creature> targets)
    {
        await CreatureCmd.GainBlock(Creature, BlockTurnBlock, null);
        return HeavySwingMove(phase);
    }

    protected override IEnumerable<AbstractIntent> HeavySwingIntent(Phase phase) =>
        [new SingleCrimsonAttackIntent(HeavySwingDamage(phase)), new StatusIntent(2)];

    [Move]
    protected override async Task<MoveState> HeavySwing(
        Phase phase,
        IReadOnlyList<Creature> targets
    )
    {
        await DamageCmd
            .Attack(HeavySwingDamage(phase))
            .FromMonster(this)
            .WithHeavySlashVfx()
            .TargetingAllOpponents(CombatState)
            .Crimson()
            .Execute(null);
        await GiveStatusCards<StaggeredCard>(
            targets,
            [new(PileType.Deck, CardPilePosition.Top), new(PileType.Discard)]
        );
        return LoopPowerupMove(phase);
    }

    protected override IEnumerable<AbstractIntent> LoopPowerupIntent(Phase phase) =>
        [new BuffIntent()];

    [Move]
    protected override async Task<MoveState> LoopPowerup(
        Phase phase,
        IReadOnlyList<Creature> targets
    )
    {
        await PowerCmd.Apply<OverchargedPower>(
            new ThrowingPlayerChoiceContext(),
            Creature,
            LoopOvercharge(phase),
            Creature,
            null
        );
        return phase == Phase.One ? ForwardSwingMove : DoubleSlashMove;
    }

    protected override IEnumerable<AbstractIntent> Phase2StunIntent => [new StunIntent()];

    [Move]
    protected override Task<MoveState> Phase2Stun(IReadOnlyList<Creature> targets) =>
        Task.FromResult(Phase2PowerupMove);

    protected override IEnumerable<AbstractIntent> Phase2PowerupIntent =>
        [new BuffIntent(), new HealIntent(), new BuffIntent()];

    [Move]
    protected override async Task<MoveState> Phase2Powerup(IReadOnlyList<Creature> targets)
    {
        await PowerUp<OverchargedPower>(Phase2Overcharge);
        await PowerUp<RegenPower>(Phase2Regen);
        await PowerUp<DexterityPower>(Phase2Dexterity);
        return DoubleSlashMove;
    }

    protected override IEnumerable<AbstractIntent> DoubleSlashIntent =>
        [new MultiAttackIntent(DoubleSwingDamage, 2), new DebuffIntent(), new StatusIntent(2)];

    [Move]
    protected override async Task<MoveState> DoubleSlash(IReadOnlyList<Creature> targets)
    {
        await DamageCmd
            .Attack(DoubleSwingDamage)
            .FromMonster(this)
            .WithHitCount(2)
            .WithSlashVfx()
            .TargetingAllOpponents(CombatState)
            .Execute(null);
        await PowerCmd.Apply<WeakPower>(
            new ThrowingPlayerChoiceContext(),
            targets,
            2,
            Creature,
            null
        );
        await GiveStatusCards<StaggeredCard>(targets, [new(PileType.Discard, Count: 2)]);
        return BlockTurnMove(Phase.Two);
    }
}

[PowerLocalization(
    title: "Overcharged",
    description: "",
    smartDescription: "At end of your turn, gain {Amount} [gold]Vigor[/gold]."
)]
file class OverchargedPower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (side != Owner.Side)
            return;

        Flash();
        await PowerCmd.Apply<VigorPower>(choiceContext, Owner, Amount, Owner, null);
    }
}

[PowerLocalization(
    title: "Unpowered",
    description: "",
    smartDescription: "When Yingzhao has {Amount} hp or less, he will be stunned."
)]
file class YingzhaoPhase2 : StunPhaseTransitionPower<YingzhaoMonster>
{
    protected override async Task OnStun()
    {
        await PowerCmd.Remove<OverchargedPower>(Owner);
        await PowerCmd.Remove<VigorPower>(Owner);
    }

    protected override MoveState GetStunState(YingzhaoMonster monster) => monster.Phase2StunMove;
}
