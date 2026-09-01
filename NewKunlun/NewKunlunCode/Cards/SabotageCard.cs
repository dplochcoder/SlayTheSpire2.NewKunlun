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
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Sabotage",
    description: "Deal {InternalDamageEnemy:diff()} [gold]Internal Damage[/gold]. Take {InternalDamageSelf:diff()} [gold]Internal Damage[/gold]."
)]
public partial class SabotageCard()
    : NewKunlunCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new InternalDamageVar(nameof(InternalDamageEnemy), 13M),
            new InternalDamageVar(nameof(InternalDamageSelf), 3M),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tips.Power<InternalDamagePower>()];

    protected override void OnUpgrade() => InternalDamageEnemy.UpgradeValueTo(19M);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await InternalDamageCmd.Apply(
            choiceContext,
            cardPlay.Target!,
            InternalDamageEnemy.BaseValue,
            Owner.Creature,
            this
        );
        await InternalDamageCmd.Apply(
            choiceContext,
            Owner.Creature,
            InternalDamageSelf.BaseValue,
            Owner.Creature,
            this
        );
    }
}
