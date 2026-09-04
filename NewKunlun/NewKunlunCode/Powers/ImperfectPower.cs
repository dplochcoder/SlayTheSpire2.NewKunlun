using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Imperfect",
    description: "Converts to [gold]Internal Damage[/gold] for each damage you take this turn."
)]
public class ImperfectPower : NewKunlunPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.InternalDamage()];

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

        var toConvert = Math.Min(result.TotalDamage, Amount);
        if (toConvert <= 0)
            return;

        await InternalDamageCmd.Inflict(
            choiceContext,
            target,
            new InternalDamageInflictVar(toConvert),
            dealer,
            cardSource,
            silent: true
        );
        await PowerCmd.ModifyAmount(
            choiceContext,
            this,
            -toConvert,
            dealer,
            cardSource,
            silent: true
        );
        Flash();
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (!participants.Contains(Owner))
            return;

        await PowerCmd.Remove(this);
        Flash();
    }
}
