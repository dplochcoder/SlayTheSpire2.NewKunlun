using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Hooks;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Tips;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Talisman Detonate",
    description: "{TotalDamage:cond:>0?Deal [green]{TotalDamage}[/green] damage.\n|}Spend {FullControl:cond:<1?up to 3 |}[gold]Qi Charges[/gold] to inflict {TalismanDetonateBaseDamage:diff()} unblockable damage per charge, and {Vulnerable:diff()} [gold]Vulnerable[/gold], to each enemy afflicted with [gold]Talisman[/gold]."
)]
public partial class TalismanDetonateCard()
    : NewKunlunCard(1, CardType.Skill, CardRarity.Basic, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new CustomVar<TalismanDetonateCard>(
                nameof(TotalDamage),
                0,
                (card, _) => card.ComputeTotalDamage()
            ),
            new CustomVar<TalismanDetonateCard>(
                nameof(FullControl),
                0,
                (card, _) => card.Owner.Creature.HasPower<FullControlPower>() ? 1 : 0
            ),
            new TalismanDetonateBaseDamageVar(10M),
            new DynamicVar(nameof(Vulnerable), 1M),
        ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Ethereal, CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [Tip.QiCharge(), Tip.Vulnerable(), Tip.Talisman()];

    private decimal ComputeTotalDamage()
    {
        if (CombatState == null || FullControl.Calculate() > 0)
            return 0;

        var modifiedDamage = ITalismanDetonateListener.ModifyTalismanDetonateBaseDamage(
            CombatState,
            TalismanDetonateBaseDamage.BaseValue,
            Owner.Creature
        );

        var charges = Owner.Creature.GetPowerAmount<QiChargePower>();
        return Math.Max(charges, 3) * modifiedDamage;
    }

    public static bool IsUpgradedAnywhere(Player? player)
    {
        var cardsSrc = player?.PlayerCombatState?.AllCards ?? player?.Deck.Cards ?? [];
        List<CardModel> cards = [.. cardsSrc];
        return cards.Any(c => c is TalismanDetonateCard)
            ? cards.Any(c => c is TalismanDetonateCard { IsUpgraded: true })
            : cards.Any(c => c is TalismanDashCard { IsUpgraded: true });
    }

    protected override bool IsPlayable =>
        Owner.Creature.GetPowerAmount<QiChargePower>() > 0
        && (CombatState?.Enemies.Any(e => e.IsHittable && e.HasTalismanFor(Owner)) ?? false);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override void OnUpgrade()
    {
        Vulnerable.UpgradeValueTo(2M);
        TalismanDetonateBaseDamage.UpgradeValueTo(15M);
    }

    public static async Task AutoPlay(
        PlayerChoiceContext choiceContext,
        Player player,
        ICombatState combatState
    )
    {
        if (player.Creature.GetPowerAmount<QiChargePower>() == 0)
            return;

        var card = player.FindCard<TalismanDetonateCard>([
            PileType.Hand,
            PileType.Deck,
            PileType.Discard,
        ]);
        if (card == null)
        {
            card = combatState.CreateCard<TalismanDetonateCard>(player);
            if (TalismanDashCard.IsUpgradedAnywhere(player))
                CardCmd.Upgrade(card);
        }

        await CardCmd.AutoPlay(choiceContext, card, null);
        await PowerCmd.Remove<TalismanDetonatePower>(player.Creature);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = cardPlay.Player.Creature;

        IReadOnlyList<Creature> eligibleCreatures =
        [
            .. CombatState!.HittableEnemies.Where(c =>
                c.GetPowerInstances<TalismanPower>().Any(p => p.Applier == player)
            ),
        ];
        if (eligibleCreatures.Count == 0)
        {
            await ClearPowers();
            return;
        }

        decimal qiCharges;
        if (Owner.Creature.GetPower<FullControlPower>() is { } fullControl)
            qiCharges = await fullControl.ConsumeQiCharges(choiceContext, Owner, this);
        else
            qiCharges = await QiChargeCmd.ConsumeQiCharges(
                choiceContext,
                Owner.Creature,
                3M,
                Owner.Creature,
                this
            );

        if (qiCharges == 0)
        {
            await ClearPowers();
            return;
        }

        await PowerCmd.Apply<VulnerablePower>(
            choiceContext,
            eligibleCreatures,
            Vulnerable.BaseValue,
            cardPlay.Player.Creature,
            null
        );
        eligibleCreatures = [.. eligibleCreatures.Where(c => c.IsHittable)];

        foreach (var creature in eligibleCreatures)
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(
                NFireSmokePuffVfx.Create(creature)
            );
        await ClearPowers();

        var modifiedDamage = ITalismanDetonateListener.ModifyTalismanDetonateBaseDamage(
            CombatState!,
            TalismanDetonateBaseDamage.BaseValue,
            Owner.Creature
        );
        var totalDamage = modifiedDamage * qiCharges;
        await CreatureCmd.Damage(
            choiceContext,
            eligibleCreatures,
            new DamageVar(totalDamage, TalismanDetonateBaseDamage.Props),
            Owner.Creature
        );
        if (Owner.Creature.CombatState == null)
            return;

        await ITalismanDetonateListener.InvokeTalismanDetonated(
            Owner.Creature.CombatState,
            choiceContext,
            (int)qiCharges,
            totalDamage,
            Owner.Creature
        );
        return;

        async Task ClearPowers()
        {
            IReadOnlyList<TalismanPower> powers =
            [
                .. CombatState?.Enemies.SelectMany(e =>
                    e.GetPowerInstances<TalismanPower>().Where(p => p.Applier == player)
                )
                    ?? [],
            ];

            foreach (var power in powers)
                await PowerCmd.Remove(power);
        }
    }
}
