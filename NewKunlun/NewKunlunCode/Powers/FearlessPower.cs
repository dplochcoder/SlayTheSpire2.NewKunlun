using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Hooks;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Fearless",
    description: "Whenever you take [gold]Internal Damage[/gold], draw {Amount} {Amount:plural:card|cards}."
)]
public class FearlessPower : NewKunlunPower, IInternalDamageListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.InternalDamage()];

    async Task IInternalDamageListener.OnInternalDamageTaken(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount,
        Creature? applier,
        CardModel? source
    )
    {
        if (target != Owner || Owner.Player == null)
            return;

        await CardPileCmd.Draw(choiceContext, Amount, Owner.Player);
        Flash();
    }
}
