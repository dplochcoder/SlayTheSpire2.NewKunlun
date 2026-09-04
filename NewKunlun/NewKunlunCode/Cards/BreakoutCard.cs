using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Breakout",
    description: "Deal {Damage:diff()} damage {IfUpgraded:[green]three times[/green]|twice} to two different random enemies."
)]
public partial class BreakoutCard()
    : NewKunlunCard(1, CardType.Attack, CardRarity.Common, TargetType.RandomEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(5M, ValueProp.Move), new DynamicVar(nameof(HitCount), 2M)];

    protected override void OnUpgrade() => HitCount.UpgradeValueTo(3M);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Creature? first = null;
        for (var i = 0; i < 2; i++)
        {
            var firstCopy = first;
            var target = CombatState!.RunState.Rng.CombatTargets.NextItem(
                CombatState!.HittableEnemies.Where(e => e != firstCopy)
            );
            if (target == null)
                return;

            await DamageCmd
                .Attack(Damage.BaseValue)
                .FromCard(this, cardPlay)
                .WithSlashVfx()
                .Targeting(target)
                .WithHitCount(HitCount.IntValue)
                .Execute(choiceContext);
            first = target;
        }
    }
}
