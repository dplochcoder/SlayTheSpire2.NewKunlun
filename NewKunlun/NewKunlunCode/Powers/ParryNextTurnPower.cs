using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Parry Next Turn",
    description: "Gain {Amount} [gold]Parry[/gold] at the end of your next turn.",
    smartDescription: "Gain {Amount} [gold]Parry[/gold] at the end {TurnsRemaining:cond:>1?your next|this} turn."
)]
public partial class ParryNextTurnPower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.Parry()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar(nameof(TurnsRemaining), 2M)];

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (!participants.Contains(Owner))
            return;
        if (--TurnsRemaining.BaseValue > 0)
        {
            Flash();
            return;
        }

        await ParryCmd.GainParry(choiceContext, Owner, Amount, Owner, null);
        await PowerCmd.Remove(this);
        Flash();
    }
}
