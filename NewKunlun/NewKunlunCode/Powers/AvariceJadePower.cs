using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Avarice Jade",
    description: "Whenever a fatal blow is dealt, gain {Amount} gold."
)]
public class AvariceJadePower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDeath(
        PlayerChoiceContext choiceContext,
        Creature creature,
        bool wasRemovalPrevented,
        float deathAnimLength
    )
    {
        if (
            Owner.Player == null
            || Owner.CombatState == null
            || !Owner.CombatState.Enemies.Contains(creature)
            || !creature.ShouldTriggerFatal()
        )
            return;

        await PlayerCmd.GainGold(Amount, Owner.Player);
        Flash();
    }
}
