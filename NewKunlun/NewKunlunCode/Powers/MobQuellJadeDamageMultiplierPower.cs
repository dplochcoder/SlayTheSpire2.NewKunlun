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
    description: "Your next {Amount:plural:{Amount}|} [gold]Detonate[/gold]{Amount:plural:s|} deal 50% more damage."
)]
public partial class MobQuellJadeDamageMultiplierPower : NewKunlunPower, ITalismanDetonateListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.Detonate()];

    decimal ITalismanDetonateListener.BaseDamageMultiplicativeModifier(
        decimal amount,
        Creature? dealer
    ) => Owner == dealer ? 1.5M : 1;

    async Task ITalismanDetonateListener.OnTalismanDetonated(
        PlayerChoiceContext choiceContext,
        int qiCharges,
        decimal totalDamage,
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
