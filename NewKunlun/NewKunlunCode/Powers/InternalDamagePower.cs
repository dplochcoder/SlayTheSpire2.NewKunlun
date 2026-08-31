using BaseLib.Hooks;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Internal Damage",
    description: "{Amount:cond:>0?{Amount} u|U}nresolved damage. If the bearer receives unblocked damage, immediately resolves to real damage. Reduces by 1 at start of turn.",
    smartDescription: ""
)]
public class InternalDamagePower : NewKunlunPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

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
                Color = new Color(0.45f, 0.1f, 0.05f),
                AffectsHpLabel = true,
                Direction = HealthBarForecastDirection.FromRight,
                LeftOriginLayout = HealthBarForecastLeftOriginLayout.Chained,
            },
        ];
    }
}
