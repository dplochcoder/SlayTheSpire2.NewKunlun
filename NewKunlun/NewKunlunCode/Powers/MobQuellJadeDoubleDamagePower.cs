using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Cards;
using NewKunlun.NewKunlunCode.Hooks;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Mob Quell Jade",
    description: "Your next {Amount} uses of {TalismanDetonate:cardName()} deal double damage."
)]
public partial class MobQuellJadeDoubleDamagePower : NewKunlunPower, ITalismanDetonateListener
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new TalismanDetonateVar<MobQuellJadeDoubleDamagePower>(power =>
                TalismanDetonateCard.IsUpgradedAnywhere(power.Owner.Player)
            ),
        ];

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        Tip.TalismanDetonateCardWithTips(Owner.Player);

    decimal ITalismanDetonateListener.BaseDamageMultiplicativeModifier(
        decimal amount,
        Creature? dealer
    ) => Owner == dealer ? 2 : 1;

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
