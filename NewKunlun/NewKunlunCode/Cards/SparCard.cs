using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
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
    title: "Spar",
    description: "Deal {Damage:diff()} damage. Gain {Block:diff()} block. Gain {Strength:diff()} [gold]Strength[/gold]. Take {InternalDamageSelfInflict:inverseDiff()} [gold]Internal Damage[/gold]."
)]
public partial class SparCard()
    : NewKunlunCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(8M, ValueProp.Move),
            new BlockVar(2M, ValueProp.Move),
            new DynamicVar(nameof(Strength), 1M),
            new InternalDamageSelfInflictVar(4M),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [Tip.Strength(), Tip.InternalDamage()];

    protected override void OnUpgrade()
    {
        Damage.UpgradeValueTo(12M);
        Block.UpgradeValueTo(3M);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd
            .Attack(Damage.BaseValue)
            .FromCard(this, cardPlay)
            .WithSlashVfx()
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);
        await CreatureCmd.GainBlock(Owner.Creature, Block, cardPlay);
        await PowerCmd.Apply<StrengthPower>(
            choiceContext,
            Owner.Creature,
            Strength.BaseValue,
            Owner.Creature,
            this
        );
        await InternalDamageCmd.Inflict(
            choiceContext,
            Owner.Creature,
            InternalDamageSelfInflict,
            Owner.Creature,
            this
        );
    }
}
