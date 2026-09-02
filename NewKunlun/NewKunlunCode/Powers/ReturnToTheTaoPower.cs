using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Hooks;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Return to the Tao",
    description: "Whenever [gold]Internal Damage[/gold] resolves on enemies this turn, immediately reapply it{Amount:cond:>1? {Amount} times|}."
)]
public class ReturnToTheTaoPower : NewKunlunPower, IInternalDamageListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (!participants.Contains(Owner))
            return;

        await PowerCmd.Remove(this);
    }

    async Task IInternalDamageListener.OnInternalDamageResolved(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount
    )
    {
        if (Owner.CombatState?.Enemies.Contains(target) ?? false)
            await InternalDamageCmd.Inflict(
                choiceContext,
                target,
                new InternalDamageInflictVar(amount),
                Owner,
                null
            );
    }
}
