using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Hooks;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Regenerate",
    description: "Whenever you [gold]Detonate[/gold], gain {Amount} {Amount:plural:[gold]Qi Charge[/gold]|[gold]Qi Charges[/gold]}."
)]
public partial class RegeneratePower : NewKunlunPower, ITalismanDetonateListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.Detonate(), Tip.QiCharge()];

    async Task ITalismanDetonateListener.OnTalismanDetonated(
        PlayerChoiceContext choiceContext,
        int qiCharges,
        decimal totalDamage,
        Creature? dealer
    )
    {
        if (Owner != dealer)
            return;

        await QiChargeCmd.GainQiCharges(choiceContext, Owner, Amount, Owner, null);
        Flash();
    }
}
