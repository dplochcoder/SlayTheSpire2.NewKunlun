using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
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
    title: "Load Shedding",
    description: "Take {InternalDamageSelfInflict:inverseDiff()} [gold]Internal Damage[/gold].\nDeal {Damage:diff()} damage.\nTransfer half of your [gold]Internal Damage[/gold] onto the enemy."
)]
public partial class LoadShed()
    : NewKunlunCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new InternalDamageSelfInflictVar(6M), new DamageVar(5M, ValueProp.Move)];

    protected override void OnUpgrade() => Damage.UpgradeValueTo(11M);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await InternalDamageCmd.Inflict(
            choiceContext,
            Owner.Creature,
            InternalDamageSelfInflict,
            Owner.Creature,
            this
        );
        await DamageCmd
            .Attack(Damage.BaseValue)
            .FromCard(this, cardPlay)
            .WithSlashVfx()
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);

        // Don't use commands for transfer.
        var power = Owner.Creature.GetPower<InternalDamagePower>();
        if (power == null || !cardPlay.Target!.IsHittable)
            return;

        var amount = (int)Math.Ceiling(power.Amount / 2M);
        await PowerCmd.ModifyAmount(choiceContext, power, -amount, Owner.Creature, this);
        var applied = await PowerCmd.Apply<InternalDamagePower>(
            choiceContext,
            cardPlay.Target!,
            amount,
            Owner.Creature,
            this
        );
        applied?.Flash();
    }
}
