using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Hooks;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Schematics",
    description: "Whenever you inflict [gold]Internal Damage[/gold] on enemies, inflict {Amount} more."
)]
public class SchematicsPower : NewKunlunPower, IInternalDamageListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tips.InternalDamage()];

    decimal IInternalDamageListener.DamageAdditiveModifier(
        Creature? target,
        decimal amount,
        Creature? applier,
        CardModel? source
    ) => applier == Owner && target != Owner ? Amount : 0;
}
