using MegaCrit.Sts2.Core.Entities.Powers;
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
    description: "{TalismanDash:cardName()} targets all enemies."
)]
public partial class MobQuellJadePower : NewKunlunPower, ITalismanDetonateListener
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new TalismanDashVar<MobQuellJadePower>(power =>
                TalismanDashCard.IsUpgradedAnywhere(power.Owner.Player)
            ),
        ];

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        Tip.TalismanDashCardWithTips(Owner.Player);
}
