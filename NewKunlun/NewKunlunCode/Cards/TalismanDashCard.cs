using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Tips;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Talisman Dash",
    description: "Deal {Damage} damage. Inflict {Weak:diff()} [gold]Weak[/gold]. Inflict [gold]Talisman[/gold]. Next turn, add a {IfUpgraded:show:[green]Talisman Detonate+[/green]|[gold]Talisman Detonate[/gold]} into your hand."
)]
public partial class TalismanDashCard()
    : NewKunlunCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(1M, ValueProp.Move), new DynamicVar(nameof(Weak), 1M), new QiChargeVar(2M)];

    public override TargetType TargetType =>
        Owner.Creature.HasPower<MobQuellJadePower>() ? TargetType.AllEnemies : TargetType.AnyEnemy;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [Tip.Weak(), Tip.Talisman(), Tip.Card<TalismanDetonateCard>(upgrade: IsUpgraded)];

    public static bool IsUpgradedAnywhere(Player? player) =>
        player != null
        && (player.PlayerCombatState?.AllCards ?? player.Deck.Cards).Any(c =>
            c is TalismanDashCard { IsUpgraded: true }
        );

    protected override void OnUpgrade()
    {
        Damage.UpgradeValueTo(3M);
        Weak.UpgradeValueTo(2M);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var attack = DamageCmd.Attack(Damage.BaseValue).FromCard(this, cardPlay);
        attack =
            TargetType == TargetType.AllEnemies
                ? attack.TargetingAllOpponents(CombatState!)
                : attack.Targeting(cardPlay.Target!);
        await attack.Execute(choiceContext);

        List<Creature> targets =
        [
            .. attack
                .Results.SelectMany(list => list)
                .Select(result => result.Receiver)
                .Distinct()
                .Where(c => c.IsHittable),
        ];
        await PowerCmd.Apply<WeakPower>(
            choiceContext,
            targets,
            Weak.BaseValue,
            Owner.Creature,
            this
        );
        await PowerCmd.Apply<TalismanPower>(choiceContext, targets, 1M, Owner.Creature, this);

        var detonatePower = await PowerCmd.Apply<TalismanDetonatePower>(
            choiceContext,
            Owner.Creature,
            1M,
            Owner.Creature,
            this
        );
        detonatePower?.Upgraded = IsUpgraded;
    }
}
