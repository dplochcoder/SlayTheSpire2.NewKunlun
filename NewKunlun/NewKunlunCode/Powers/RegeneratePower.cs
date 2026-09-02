using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using NewKunlun.NewKunlunCode.Hooks;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Powers;

[CardLocalization(
    title: "Regenerate",
    description: "Whenever you play [gold]Talisman Detonate[/gold], gain {Amount:plural:[gold]Qi Charges[/gold]|[gold]Qi Charge[/gold]}."
)]
public class RegeneratePower : NewKunlunPower, ITalismanDetonateListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    async Task ITalismanDetonateListener.OnTalismanDetonated(
        PlayerChoiceContext choiceContext,
        decimal amount,
        Creature? dealer
    )
    {
        if (Owner != dealer)
            return;

        await QiChargeCmd.GainQiCharges(choiceContext, Owner, Amount, Owner, null);
        Flash();
    }
}
