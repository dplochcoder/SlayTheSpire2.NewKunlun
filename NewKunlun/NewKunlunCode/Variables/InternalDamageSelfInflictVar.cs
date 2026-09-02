using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace NewKunlun.NewKunlunCode.Variables;

public class InternalDamageSelfInflictVar(string name, decimal damage)
    : InternalDamageInflictVar(name, damage)
{
    public InternalDamageSelfInflictVar(decimal damage)
        : this("InternalDamageSelfInflict", damage) { }

    protected override Creature? ModifyTarget(CardModel card, Creature? origTarget) =>
        card.Owner.Creature;
}
