using BaseLib.Hooks;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace SlayTheSpire2.NewKunlun.SlayTheSpire2.NewKunlunCode.Powers;

public class InternalDamagePower : NewKunlunPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public static async Task Apply(
        PlayerChoiceContext choiceContext,
        Creature target,
        Decimal amount,
        Creature? applier,
        CardModel? cardSource,
        bool silent = false
    )
    {
        // TODO: Hook modifiers.
        await PowerCmd.Apply<InternalDamagePower>(
            choiceContext,
            target,
            amount,
            applier,
            cardSource,
            silent
        );
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (!participants.Contains(Owner))
            return;

        await PowerCmd.Decrement(this);
        Flash();
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
        if (target != Owner || result.UnblockedDamage <= 0)
            return;

        var damage = Amount;
        await PowerCmd.Remove(this);
        Flash();
        await CreatureCmd.Damage(
            choiceContext,
            Owner,
            damage,
            ValueProp.Unblockable | ValueProp.Unpowered,
            Owner
        );
    }

    public override IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(
        HealthBarForecastContext context
    )
    {
        return
        [
            new HealthBarForecastSegment()
            {
                Amount = Amount,
                Color = new Color(0.6f, 0.1f, 0.1f),
                AffectsHpLabel = true,
                Direction = HealthBarForecastDirection.FromRight,
                LeftOriginLayout = HealthBarForecastLeftOriginLayout.Chained,
            },
        ];
    }
}
