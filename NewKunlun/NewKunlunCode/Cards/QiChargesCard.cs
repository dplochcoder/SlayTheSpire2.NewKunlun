using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Cards;

// Not a playable card. Used for custom dialogues, like Full Control.
[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Qi Charges",
    description: "[gold]Discharge[/gold] {QiCharges}.\n[gold]Detonate[/gold] {DetonateDamage}."
)]
public partial class QiChargesCard()
    : NewKunlunCard(1, CardType.None, CardRarity.Token, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new(nameof(QiCharges), 0M), new(nameof(DetonateDamage), 0M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<QiChargePower>()];
}
