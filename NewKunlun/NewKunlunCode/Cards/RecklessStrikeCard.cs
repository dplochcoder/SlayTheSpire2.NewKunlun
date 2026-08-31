using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Reckless Strike",
    description: "Deal {Damage:diff()} damage. Take {InternalDamage:diff()} [gold]Internal Damage[/gold]."
)]
public partial class RecklessStrikeCard()
    : NewKunlunCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(16M, ValueProp.Move), new InternalDamageVar(8M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tips.Power<InternalDamagePower>()];

    protected override void OnUpgrade() => Damage.UpgradeValueTo(21M);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd
            .Attack(Damage.BaseValue)
            .FromCard(this, cardPlay)
            .WithSlashVfx()
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);
        await InternalDamageCmd.Apply(
            choiceContext,
            Owner.Creature,
            InternalDamage.BaseValue,
            Owner.Creature,
            this
        );
    }
}
