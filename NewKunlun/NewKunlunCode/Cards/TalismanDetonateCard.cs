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
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Hooks;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Talisman Detonate",
    description: "Spend up to {QiCharge:diff()} [gold]Qi Charges[/gold] to inflict {TalismanDetonateDamage:diff()} unblockable damage per charge, and {Vulnerable:diff()} [gold]Vulnerable[/gold], to each enemy afflicted with [gold]Talisman[/gold]."
)]
public partial class TalismanDetonateCard()
    : NewKunlunCard(2, CardType.Skill, CardRarity.Basic, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new QiChargeVar(3M),
            new DynamicVar(nameof(Vulnerable), 2M),
            new TalismanDetonateDamageVar(14M),
        ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Ethereal, CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [Tips.Power<QiChargePower>(), Tips.Power<VulnerablePower>(), Tips.Power<TalismanPower>()];

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
        Vulnerable.UpgradeValueTo(3M);
        TalismanDetonateDamage.UpgradeValueTo(20M);
    }

    public static async Task AutoPlay(
        PlayerChoiceContext choiceContext,
        Player player,
        ICombatState combatState
    )
    {
        if (player.Creature.GetPowerAmount<QiChargePower>() == 0)
            return;

        List<PileType> pileTypes = [PileType.Hand, PileType.Draw, PileType.Discard];
        CardModel? card = null;
        foreach (var pileType in pileTypes)
        {
            List<TalismanDetonateCard> cards =
            [
                .. pileType.GetPile(player).Cards.OfType<TalismanDetonateCard>(),
            ];
            card ??= cards.FirstOrDefault(c => c.IsUpgraded);
            card ??= cards.FirstOrDefault();
            if (card != null)
                break;
        }

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

        decimal charges;
        if (Owner.Creature.GetPower<FullControlPower>() is { } fullControl)
            charges = await fullControl.ConsumeQiCharges(choiceContext, Owner, this);
        else
            charges = await QiChargeCmd.ConsumeQiCharges(
                choiceContext,
                Owner.Creature,
                QiCharge.BaseValue,
                Owner.Creature,
                this
            );

        if (charges == 0)
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

        var modifiedDamage = ITalismanDetonateListener.ModifyTalismanDetonateDamage(
            CombatState!,
            TalismanDetonateDamage.BaseValue,
            Owner.Creature
        );
        await CreatureCmd.Damage(
            choiceContext,
            eligibleCreatures,
            new DamageVar(modifiedDamage * charges, TalismanDetonateDamage.Props),
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
