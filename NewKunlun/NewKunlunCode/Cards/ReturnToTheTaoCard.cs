using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Return to the Tao",
    description: "Whenever [gold]Internal Damage[/gold] resolves on enemies this turn, immediately reapply it. Deal {Damage:diff()} damage to all enemies {Repeats:diff()} times."
)]
public partial class ReturnToTheTaoCard()
    : NewKunlunCard(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(1M, ValueProp.Move), new DynamicVar(nameof(Repeats), 3M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tips.InternalDamage()];

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ReturnToTheTaoPower>(
            choiceContext,
            Owner.Creature,
            1M,
            Owner.Creature,
            this
        );
        await DamageCmd
            .Attack(Damage.BaseValue)
            .FromCard(this, cardPlay)
            .TargetingAllOpponents(Owner.Creature.CombatState!)
            .WithHitCount((int)Repeats.BaseValue)
            .Execute(choiceContext);
    }
}
