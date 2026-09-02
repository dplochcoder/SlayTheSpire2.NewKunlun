using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
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
            new InternalDamageInflictVar(nameof(InternalDamageEnemy), 13M),
            new InternalDamageSelfInflictVar(nameof(InternalDamageSelf), 3M),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tips.InternalDamage()];

    protected override void OnUpgrade() => InternalDamageEnemy.UpgradeValueTo(19M);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await InternalDamageCmd.Inflict(
            choiceContext,
            cardPlay.Target!,
            InternalDamageEnemy,
            Owner.Creature,
            this
        );
        await InternalDamageCmd.Inflict(
            choiceContext,
            Owner.Creature,
            InternalDamageSelf,
            Owner.Creature,
            this
        );
    }
}
