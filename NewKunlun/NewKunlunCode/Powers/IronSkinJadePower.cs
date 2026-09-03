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
    title: "Iron Skin Jade",
    description: "When receiving unblocked damage, convert up to {Amount} to [gold]Internal Damage[/gold]."
)]
public class IronSkinJadePower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private decimal _convertedDamage = 0;

    public override decimal ModifyHpLostAfterOsty(
        Creature target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource
    )
    {
        if (target != Owner)
            return amount;

        var converted = Math.Min(amount, Amount);
        _convertedDamage += converted;
        return amount - converted;
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
        while (_convertedDamage > 0)
        {
            var toConvert = _convertedDamage;
            _convertedDamage = 0;
            await InternalDamageCmd.Inflict(
                choiceContext,
                Owner,
                new InternalDamageInflictVar(toConvert),
                Owner,
                null
            );
        }
    }
}
