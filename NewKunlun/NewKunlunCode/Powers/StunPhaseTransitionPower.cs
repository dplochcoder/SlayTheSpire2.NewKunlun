using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.ValueProps;

namespace NewKunlun.NewKunlunCode.Powers;

public abstract class StunPhaseTransitionPower<TMonster> : NewKunlunPower
    where TMonster : MonsterModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.Static(StaticHoverTip.Stun)];

    protected abstract MoveState GetStunState(TMonster monster);

    protected virtual Task OnStun() => Task.CompletedTask;

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource
    )
    {
        if (Owner.Monster is not TMonster monster || target != Owner || Owner.CurrentHp > Amount)
            return;

        await OnStun();
        monster.SetMoveImmediate(GetStunState(monster));
        await PowerCmd.Remove(this);
    }
}
