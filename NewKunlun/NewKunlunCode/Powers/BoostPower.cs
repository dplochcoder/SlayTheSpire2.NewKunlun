using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using NewKunlun.NewKunlunCode.Hooks;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Boost",
    description: "Deal {Amount} more damage per [gold]Discharge[/gold] when you [gold]Detonate[/gold]."
)]
public class BoostPower : NewKunlunPower, ITalismanDetonateListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    decimal ITalismanDetonateListener.BaseDamageAdditiveModifier(
        decimal amount,
        Creature? dealer
    ) => Owner == dealer ? Amount : 0;
}
