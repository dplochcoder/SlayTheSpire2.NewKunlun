using MegaCrit.Sts2.Core.Entities.Creatures;

namespace NewKunlun.NewKunlunCode.Variables;

public class MedicinePipeRemainingUsesVar()
    : CustomVar<Cards.MedicinePipeCard>("MedicinePipeRemainingUses")
{
    protected override decimal Calculate(Cards.MedicinePipeCard owner, Creature? target) =>
        Math.Max(0, owner.TotalUses.BaseValue - owner.TimesUsed);
}
