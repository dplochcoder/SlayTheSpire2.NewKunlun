using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
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
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Talisman Dash",
    description: "Deal {Damage} damage. Inflict {Weak:diff()} [gold]Weak[/gold]. Spend up to {QiCharge:diff()} [gold]Qi Charges[/gold], inflict one [gold]Talisman[/gold] per change. Next turn, add a {IfUpgraded:show:[green]Talisman Detonate+[/green]:[gold]Talisman Detonate[/gold]} into your hand."
)]
public partial class TalismanDashCard()
    : NewKunlunCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(1M, ValueProp.Move), new DynamicVar(nameof(Weak), 1M), new QiChargeVar(2M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<WeakPower>(),
            HoverTipFactory.FromPower<QiChargePower>(),
            HoverTipFactory.FromPower<TalismanPower>(),
            HoverTipFactory.FromCard<TalismanDetonateCard>(upgrade: IsUpgraded),
        ];

    protected override bool ShouldGlowGoldInternal =>
        Owner.Creature.GetPowerAmount<QiChargePower>() > 0;

    public static bool IsUpgradedAnywhere(Player? player) =>
        player != null
        && (player.PlayerCombatState?.AllCards ?? player.Deck.Cards).Any(c =>
            c is TalismanDashCard or TalismanDetonateCard && c.IsUpgraded
        );

    protected override void OnUpgrade()
    {
        Weak.UpgradeValueTo(2M);
        QiCharge.UpgradeValueTo(2M);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd
            .Attack(Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);
        await PowerCmd.Apply<WeakPower>(
            choiceContext,
            cardPlay.Target!,
            Weak.BaseValue,
            Owner.Creature,
            this
        );
        var charges = await QiChargeCmd.ConsumeQiCharges(
            choiceContext,
            Owner.Creature,
            QiCharge.BaseValue,
            Owner.Creature,
            this
        );
        if (charges == 0)
            return;

        await PowerCmd.Apply<TalismanPower>(
            choiceContext,
            cardPlay.Target!,
            charges,
            Owner.Creature,
            this
        );
        var power = await PowerCmd.Apply<TalismanDetonatePower>(
            choiceContext,
            Owner.Creature,
            1M,
            Owner.Creature,
            this
        );
        power?.Upgraded = IsUpgraded;
    }
}
