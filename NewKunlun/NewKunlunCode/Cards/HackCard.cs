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
    description: "Inflict {InternalDamageInflict:diff()} [gold]Internal Damage[/gold]. Inflict [gold]Internal Damage[/gold] equal to their current total."
)]
public partial class HackCard()
    : NewKunlunCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new InternalDamageInflictVar(4M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.InternalDamage()];

    protected override void OnUpgrade() => InternalDamageInflict.UpgradeValueTo(7M);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target!;
        await InternalDamageCmd.Inflict(
            choiceContext,
            target,
            InternalDamageInflict,
            Owner.Creature,
            this
        );

        var total = target.GetPowerAmount<InternalDamagePower>();
        await InternalDamageCmd.Inflict(
            choiceContext,
            target,
            new InternalDamageInflictVar(total),
            Owner.Creature,
            this
        );
    }
}
