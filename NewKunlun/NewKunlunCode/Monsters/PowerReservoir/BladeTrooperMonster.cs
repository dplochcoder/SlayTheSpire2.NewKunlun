using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Cards.Statuses;
using NewKunlun.NewKunlunCode.Extensions;

namespace NewKunlun.NewKunlunCode.Monsters.PowerReservoir;

[Monster]
public class BladeTrooperMonster : BladeTrooperMonsterBase
{
    public override int MaxInitialHp => AscensionLevel.ToughEnemies.Get(38, 35);

    private static int DoubleSwingDamage => AscensionLevel.ToughEnemies.Get(5, 4);

    protected override IEnumerable<AbstractIntent> DoubleSwingIntent =>
        [new MultiAttackIntent(DoubleSwingDamage, 2)];

    [InitialMove]
    protected override async Task<MoveState> DoubleSwing(IReadOnlyList<Creature> targets)
    {
        await DamageCmd
            .Attack(DoubleSwingDamage)
            .FromMonster(this)
            .WithHitCount(2)
            .WithSlashVfx()
            .TargetingAllOpponents(CombatState)
            .Execute(null);
        return SwingAndBlockMove;
    }

    private static int SwingAndBlockDamage => AscensionLevel.DeadlyEnemies.Get(8, 6);

    private static BlockVar SwingAndBlockBlock => new(6, ValueProp.Move);

    protected override IEnumerable<AbstractIntent> SwingAndBlockIntent =>
        [new SingleAttackIntent(SwingAndBlockDamage), new DefendIntent()];

    [Move]
    protected override async Task<MoveState> SwingAndBlock(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(SwingAndBlockDamage).FromMonster(this).WithSlashVfx().Execute(null);
        await CreatureCmd.GainBlock(Creature, SwingAndBlockBlock, null);
        return BuffMove;
    }

    private static int BuffStrength => AscensionLevel.DeadlyEnemies.Get(3, 2);

    protected override IEnumerable<AbstractIntent> BuffIntent => [new BuffIntent()];

    [Move]
    protected override async Task<MoveState> Buff(IReadOnlyList<Creature> targets)
    {
        await PowerCmd.Apply<StrengthPower>(
            new ThrowingPlayerChoiceContext(),
            Creature,
            BuffStrength,
            Creature,
            null
        );
        return SwingAndStaggerMove;
    }

    private static int SwingAndStaggerDamage => AscensionLevel.DeadlyEnemies.Get(4, 3);

    protected override IEnumerable<AbstractIntent> SwingAndStaggerIntent =>
        [new SingleAttackIntent(SwingAndBlockDamage), new StatusIntent(1)];

    [Move]
    protected override async Task<MoveState> SwingAndStagger(IReadOnlyList<Creature> targets)
    {
        await DamageCmd
            .Attack(SwingAndStaggerDamage)
            .FromMonster(this)
            .WithSlashVfx()
            .TargetingAllOpponents(CombatState)
            .Execute(null);
        await CardPileCmd.AddToCombatAndPreview<StaggeredCard>(
            targets,
            PileType.Draw,
            1,
            null,
            CardPilePosition.Top
        );
        return DoubleSwingMove;
    }
}
