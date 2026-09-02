using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Hooks;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Mending",
    description: "Whenever you would take [gold]Internal Damage[/gold], take {Amount} less."
)]
public class MendingPower : NewKunlunPower, IInternalDamageListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    decimal IInternalDamageListener.DamageAdditiveModifier(
        Creature? target,
        decimal amount,
        Creature? applier,
        CardModel? source
    ) => (target == Owner) ? -Amount : 0;
}
