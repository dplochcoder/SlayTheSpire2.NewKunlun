using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Avarice Jade",
    description: "Whenever a [gold]Fatal[/gold] blow is dealt, gain {Amount} gold."
)]
public class AvariceJadePower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.Fatal()];

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
