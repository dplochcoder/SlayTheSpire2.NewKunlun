using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    "Talisman Dash",
    "Deal {Damage} damage. Inflict {Weak:diff()} [gold]Weak[/gold]. Spend up to {QiCharge:diff()} Qi Charges, inflict one [gold]Talisman[/gold] per change. Next turn, add a {IfUpgraded:show:[green]Talisman Detonate+[/green]:[gold]Talisman Detonate[/gold]} into your hand."
)]
public partial class TalismanDashCard()
    : NewKunlunCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(1M, ValueProp.Move), new DynamicVar("Weak", 1M), new QiChargeVar(3M)];
}
