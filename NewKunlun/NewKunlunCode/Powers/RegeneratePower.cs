using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Cards;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Hooks;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Regenerate",
    description: "Whenever you play {TalismanDetonate:cardName}, gain {Amount} {Amount:plural:[gold]Qi Charges[/gold]|[gold]Qi Charge[/gold]}."
)]
public partial class RegeneratePower : NewKunlunPower, ITalismanDetonateListener
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new TalismanDetonateVar<RegeneratePower>(power =>
                TalismanDetonateCard.IsUpgradedAnywhere(power.Owner.Player)
            ),
        ];

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

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
