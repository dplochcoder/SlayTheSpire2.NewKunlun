using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Patience",
    description: "Gain {Block:diff()} [gold]Block[/gold].\nHeal {InternalDamageHeal:diff()} [gold]Internal Damage[/gold]."
)]
public partial class PatienceCard()
    : NewKunlunCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(6M, ValueProp.Move), new InternalDamageHealVar(5M)];

    protected override void OnUpgrade()
    {
        Block.UpgradeValueTo(9M);
        InternalDamageHeal.UpgradeValueTo(7M);
    }
}
