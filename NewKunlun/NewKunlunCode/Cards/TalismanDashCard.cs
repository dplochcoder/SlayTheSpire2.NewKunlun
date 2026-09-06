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
    description: "{MobQuellJade:cond:>0?Targets all enemies.\n}Deal {Damage} damage.{IfUpgraded:show: [gold]Boost[/gold] {Boost:diff()}.|}\nInflict {Weak:diff()} [gold]Weak[/gold].\n[gold]Mark[/gold]. Next turn, add {TalismanDetonate:cardName()} into your hand."
)]
public partial class TalismanDashCard()
    : NewKunlunCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(6M, ValueProp.Move),
            new DynamicVar(nameof(Weak), 1M),
            new DynamicVar(nameof(Boost), 0M),
            new CardNameVar<TalismanDetonateCard>(() => IsUpgraded),
            new CustomVar(
                nameof(MobQuellJade),
                0M,
                _ => Owner.Creature.HasPower<MobQuellJadePower>() ? 1 : 0
            ),
        ];

    public override TargetType TargetType =>
        Owner.Creature.HasPower<MobQuellJadePower>() ? TargetType.AllEnemies : TargetType.AnyEnemy;

    private IEnumerable<IHoverTip> BoostTip() => IsUpgraded ? [Tip.Boost()] : [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            Tip.Weak(),
            .. BoostTip(),
            Tip.Mark(),
            Tip.Card<TalismanDetonateCard>(upgrade: IsUpgraded),
        ];

    public static bool IsUpgradedAnywhere(Player? player) =>
        player != null
        && (player.PlayerCombatState?.AllCards ?? player.Deck.Cards).Any(c =>
            c is TalismanDashCard { IsUpgraded: true }
        );

    protected override void OnUpgrade()
    {
        Damage.UpgradeValueTo(3M);
        Weak.UpgradeValueTo(2M);
        Boost.UpgradeValueTo(2M);
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
        await TalismanPower.RemoveAll(Owner);
        await PowerCmd.Apply<TalismanPower>(choiceContext, targets, 1M, Owner.Creature, this);

        var detonatePower = await PowerCmd.Apply<TalismanDetonatePower>(
            choiceContext,
            Owner.Creature,
            1M,
            Owner.Creature,
            this
        );
        detonatePower?.IsUpgraded = IsUpgraded;
    }
}
