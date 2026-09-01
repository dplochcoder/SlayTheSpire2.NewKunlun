using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Skull Kick",
    description: "Deal {Damage:diff()} damage. Inflict {Weak:diff()} [glow]Weak[/glow]. If the enemy intends to attack, it loses {StrengthLoss:diff()} [glow]Strength[/glow]."
)]
public partial class SkullKickCard()
    : NewKunlunCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(14M, ValueProp.Move),
            new DynamicVar(nameof(Weak), 1M),
            new DynamicVar(nameof(StrengthLoss), 1M),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [Tips.Power<WeakPower>(), Tips.Power<StrengthPower>()];

    protected override bool ShouldGlowGoldInternal =>
        CombatState?.Enemies.Any(e => e.Monster?.IntendsToAttack ?? false) ?? false;

    protected override void OnUpgrade()
    {
        Damage.UpgradeValueTo(18M);
        Weak.UpgradeValueTo(2M);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var intendedToAttack = cardPlay.Target?.Monster?.IntendsToAttack ?? false;

        await DamageCmd
            .Attack((decimal)Damage.BaseValue)
            .FromCard(this, cardPlay)
            .WithSlashVfx()
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);
        await PowerCmd.Apply<WeakPower>(
            choiceContext,
            cardPlay.Target!,
            Weak.BaseValue,
            Owner.Creature,
            this
        );
        if (intendedToAttack)
            await PowerCmd.Apply<StrengthPower>(
                choiceContext,
                cardPlay.Target!,
                -StrengthLoss.BaseValue,
                Owner.Creature,
                this
            );
    }
}
