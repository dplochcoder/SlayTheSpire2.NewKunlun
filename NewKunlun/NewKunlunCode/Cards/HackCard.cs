using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Tips;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Hack",
    description: "Inflict {BaseDamage:diff()} [gold]Internal Damage[/gold]. Inflict [gold]Internal Damage[/gold] {IfUpgraded:show:[green]double[/green]|equal to} their current total."
)]
public partial class HackCard()
    : NewKunlunCard(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new InternalDamageInflictVar(nameof(BaseDamage), 4M),
            new DynamicVar(nameof(Multiplier), 2M),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.InternalDamage()];

    protected override void OnUpgrade()
    {
        BaseDamage.UpgradeValueTo(5M);
        Multiplier.UpgradeValueTo(3M);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await InternalDamageCmd.Inflict(
            choiceContext,
            cardPlay.Target!,
            BaseDamage,
            Owner.Creature,
            this
        );

        var total = cardPlay.Target!.GetPowerAmount<InternalDamagePower>();
        var toApply = total * Multiplier.BaseValue - total;
        await InternalDamageCmd.Inflict(
            choiceContext,
            cardPlay.Target!,
            new InternalDamageInflictVar(toApply),
            Owner.Creature,
            this
        );
    }
}
