using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Ricochet Jade",
    description: "Every time you fully block an attack, inflict {Amount} [gold]Internal Damage[/gold] to the attacker."
)]
public class RicochetJadePower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource
    )
    {
        if (
            target != Owner
            || !result.WasFullyBlocked
            || dealer == null
            || !props.IsPoweredAttack()
        )
            return;

        await InternalDamageCmd.Inflict(
            choiceContext,
            dealer,
            new InternalDamageInflictVar(Amount),
            Owner,
            null
        );
    }
}
