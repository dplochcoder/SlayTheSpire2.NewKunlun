using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    "Parry",
    "If you are hit by the enemy this turn, {InternalDamage:cond:>0?take {InternalDamage} [gold]Internal Damage[/gold] and |}gain {Amount:plural:[gold]Qi Charge[/gold]:[gold]Qi Charges[/gold]}.",
    ""
)]
public partial class ParryPower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new InternalDamageVar(0M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            if (InternalDamage.BaseValue > 0)
                yield return InternalDamagePower.HoverTip();
            yield return QiChargePower.HoverTip();
        }
    }

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource
    )
    {
        if (target != Owner || !props.IsPoweredAttack())
            return;

        await InternalDamageCmd.Apply(choiceContext, target, InternalDamage, Owner, null);
        await QiChargeCmd.AddQiCharges(choiceContext, target, Amount, Owner, null);
        await PowerCmd.Remove(this);
        Flash();
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (participants.Contains(Owner))
        {
            await PowerCmd.Remove(this);
            Flash();
        }
    }
}
