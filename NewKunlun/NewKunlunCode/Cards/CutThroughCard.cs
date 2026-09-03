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
using NewKunlun.NewKunlunCode.Tips;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Cut Through",
    description: "Deal {Damage:diff()} damage. Inflict {InternalDamageInflict:diff()} [gold]Internal Damage[/gold]."
)]
public partial class CutThroughCard()
    : NewKunlunCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(3M, ValueProp.Move), new InternalDamageInflictVar(7M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.InternalDamage()];

    protected override void OnUpgrade()
    {
        Damage.UpgradeValueTo(6M);
        InternalDamageInflict.UpgradeValueTo(9M);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd
            .Attack(Damage.BaseValue)
            .FromCard(this, cardPlay)
            .WithSlashVfx()
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);
        await InternalDamageCmd.Inflict(
            choiceContext,
            cardPlay.Target!,
            InternalDamageInflict,
            Owner.Creature,
            this
        );
    }
}
