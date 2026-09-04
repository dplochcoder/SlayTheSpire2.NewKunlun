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

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Tai-Chi Kick",
    description: "Deal {Damage:diff()} damage. Gain {Block:diff()} [gold]Block[/gold]. If the enemy intends to attack, gain 1 [gold]Qi Charge[/gold]."
)]
public partial class TaiChiKickCard()
    : NewKunlunCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(4M, ValueProp.Move), new BlockVar(4M, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.QiCharge()];

    protected override void OnUpgrade()
    {
        Damage.UpgradeValueTo(6M);
        Block.UpgradeValueTo(7M);
    }

    protected override bool ShouldGlowGoldInternal =>
        CombatState?.Enemies.Any(e => e.IsHittable && (e.Monster?.IntendsToAttack ?? false))
        ?? false;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var intendedToAttack = cardPlay.Target?.Monster?.IntendsToAttack ?? false;
        await DamageCmd
            .Attack(Damage.BaseValue)
            .FromCard(this, cardPlay)
            .WithSlashVfx()
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);
        await CreatureCmd.GainBlock(Owner.Creature, Block, cardPlay);
        if (intendedToAttack)
            await QiChargeCmd.GainQiCharges(
                choiceContext,
                Owner.Creature,
                1M,
                Owner.Creature,
                this
            );
    }
}
