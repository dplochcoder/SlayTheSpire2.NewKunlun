using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using NewKunlun.NewKunlunCode.Hooks;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Mob Quell Jade",
    description: "Your next {Amount} uses of [gold]Talisman Detonate[/gold] deal double damage."
)]
public class MobQuellJadeDoubleDamagePower : NewKunlunPower, ITalismanDetonateListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        Tip.TalismanDetonateCardWithTips(Owner.Player);

    decimal ITalismanDetonateListener.DamageMultiplicativeModifier(
        decimal amount,
        Creature? dealer
    ) => Owner == dealer ? 2 : 1;

    async Task ITalismanDetonateListener.OnTalismanDetonated(
        PlayerChoiceContext choiceContext,
        decimal amount,
        Creature? dealer
    )
    {
        if (Owner == dealer)
        {
            await PowerCmd.Decrement(this);
            Flash();
        }
    }
}
