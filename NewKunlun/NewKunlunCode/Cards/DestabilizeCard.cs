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
using NewKunlun.NewKunlunCode.Tips;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Destabilize",
    description: "Inflict {Imperfect:diff()} [gold]Imperfect[/gold]. Deal {Damage:diff()} damage. Take {InternalDamageSelfInflict:inverseDiff()} [gold]Internal Damage[/gold]."
)]
public partial class DestabilizeCard()
    : NewKunlunCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DynamicVar(nameof(Imperfect), 20M),
            new DamageVar(4M, ValueProp.Move),
            new InternalDamageSelfInflictVar(5M),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [Tip.Imperfect(), Tip.InternalDamage()];

    protected override void OnUpgrade()
    {
        Damage.UpgradeValueTo(7M);
        Imperfect.UpgradeValueTo(30M);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ImperfectPower>(
            choiceContext,
            cardPlay.Target!,
            Imperfect.BaseValue,
            Owner.Creature,
            this
        );
        await DamageCmd
            .Attack(Damage.BaseValue)
            .FromCard(this, cardPlay)
            .WithHeavySlashVfx()
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);
        await InternalDamageCmd.Inflict(
            choiceContext,
            Owner.Creature,
            InternalDamageSelfInflict,
            Owner.Creature,
            this
        );
    }
}
