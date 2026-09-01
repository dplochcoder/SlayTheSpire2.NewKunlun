using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Cards;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Shadow Strike",
    description: "Deal {Damage:diff()} damage. Hits twice if the enemy is not attacking this turn. Thrice if the enemy is stunned."
)]
public partial class ShadowStrikeCard()
    : NewKunlunCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(13M, ValueProp.Move)];

    protected override void OnUpgrade() => Damage.UpgradeValueTo(18M);

    protected override bool ShouldGlowGoldInternal =>
        CombatState?.Enemies.Any(e => e.IsStunned || e.Monster is { IntendsToAttack: false })
        ?? false;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target!;
        int times =
            target.IsStunned ? 3
            : target.Monster is { IntendsToAttack: false } ? 2
            : 1;

        await DamageCmd
            .Attack(Damage.BaseValue)
            .FromCard(this, cardPlay)
            .WithSlashVfx()
            .WithHitCount(times)
            .Targeting(target)
            .Execute(choiceContext);
    }
}
