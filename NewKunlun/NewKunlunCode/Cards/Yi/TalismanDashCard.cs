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
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Tips;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards.Yi;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Talisman Dash",
    description: "{MobQuellJade:cond:>0?Targets all enemies.\n|}[gold]Lock[/gold] 1. Deal {Damage:diff()} damage.\n{IfUpgraded:show:[gold]Boost[/gold] {Boost:diff()}.\n|}Inflict {Weak:diff()} [gold]Weak[/gold].\n[gold]Mark[/gold]. Next turn, add {TalismanDetonate:cardName()} into your hand."
)]
public partial class TalismanDashCard()
    : NewKunlunCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(1M, ValueProp.Move),
            new(nameof(Weak), 1M),
            new(nameof(Boost), 0M),
            new CardNameVar<TalismanDetonateCard>(() => IsUpgraded),
            new CustomVar(
                nameof(MobQuellJade),
                0M,
                _ => Owner.Creature.HasPower<MobQuellJadePower>() ? 1 : 0
            ),
        ];

    private bool HasMobQuellJade =>
        IsMutable && Owner != null && Owner.Creature.HasPower<MobQuellJadePower>();

    public override TargetType TargetType =>
        HasMobQuellJade ? TargetType.AllEnemies : TargetType.AnyEnemy;

    private IEnumerable<IHoverTip> BoostTip() =>
        IsUpgraded ? [HoverTipFactory.Static(CustomStaticHoverTip.Boost)] : [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.Static(CustomStaticHoverTip.Lock),
            HoverTipFactory.FromPower<QiChargePower>(),
            HoverTipFactory.FromPower<WeakPower>(),
            .. BoostTip(),
            HoverTipFactory.Static(CustomStaticHoverTip.Mark),
            HoverTipFactory.FromCard<TalismanDetonateCard>(upgrade: IsUpgraded),
        ];

    protected override bool IsPlayable =>
        QiChargeCmd.GetAvailable(Owner.Creature, QiChargeCmd.Locked.IncludeLocked) >= 1;

    protected override bool ShouldGlowRedInternal => !IsPlayable;

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    public static bool IsUpgradedAnywhere(Player? player) =>
        player != null
        && (player.PlayerCombatState?.AllCards ?? player.Deck.Cards).Any(c =>
            c is TalismanDashCard { IsUpgraded: true }
        );

    public static IHoverTip CreateHoverTip(Player? player) =>
        HoverTipFactory.FromCard<TalismanDashCard>(upgrade: IsUpgradedAnywhere(player));

    public static IEnumerable<IHoverTip> CreateHoverTips(Player? player) =>
        HoverTipFactory.FromCardWithCardHoverTips<TalismanDashCard>(
            upgrade: IsUpgradedAnywhere(player)
        );

    protected override void OnUpgrade()
    {
        Damage.UpgradeValueTo(6M);
        Weak.UpgradeValueTo(2M);
        Boost.UpgradeValueTo(2M);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var locked = QiChargeCmd.Lock(Owner.Creature, 1);
        if (!locked)
            return;

        if (CombatState == null)
            return;
        var attack = DamageCmd.Attack((decimal)Damage.BaseValue).FromCard(this, cardPlay);
        attack = HasMobQuellJade
            ? attack.TargetingAllOpponents(CombatState)
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
