using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Hooks;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Neural Network",
    description: "Whenever you take [gold]Internal Damage[/gold], gain that much block."
)]
public class NeuralNetworkPower : NewKunlunPower, IInternalDamageListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    async Task IInternalDamageListener.OnInternalDamageTaken(
        Creature target,
        decimal amount,
        Creature? applier,
        CardModel? cardSource
    )
    {
        if (target != Owner)
            return;
        await CreatureCmd.GainBlock(
            target,
            new BlockVar(amount * Amount, ValueProp.Unpowered),
            null
        );
    }
}
