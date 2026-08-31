using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Talisman Detonate",
    description: "Inflict {Vulnerable} [gold]Vulnerable[/gold] to all [gold]Talisman[/gold] targets. Targets take {Damage} unblockable damage per spent [gold]Qi Charge[/gold]."
)]
public partial class TalismanDetonateCard()
    : NewKunlunCard(2, CardType.Skill, CardRarity.Basic, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(13M, ValueProp.Unblockable | ValueProp.Unpowered),
            new DynamicVar(nameof(Vulnerable), 1M),
        ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Ethereal, CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<VulnerablePower>(),
            HoverTipFactory.FromPower<TalismanPower>(),
            HoverTipFactory.FromPower<QiChargePower>(),
        ];

    protected override bool ShouldGlowGoldInternal =>
        CombatState?.Enemies.Any(e =>
            e.IsHittable
            && e.GetPowerInstances<TalismanPower>().Any(p => p.Applier == Owner.Creature)
        )
        ?? false;

    protected override void OnUpgrade()
    {
        Damage.UpgradeValueTo(18M);
        Vulnerable.UpgradeValueTo(2M);
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
            return;

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

        foreach (var creature in eligibleCreatures)
        {
            IReadOnlyList<TalismanPower> powers =
            [
                .. creature.GetPowerInstances<TalismanPower>().Where(p => p.Applier == player),
            ];
            int charges = powers.Select(p => p.Amount).Sum();
            await CreatureCmd.Damage(
                choiceContext,
                creature,
                new DamageVar(charges * Damage.BaseValue, Damage.Props),
                this,
                cardPlay
            );

            foreach (var power in powers)
            {
                await PowerCmd.Remove(power);
                power.Flash();
            }
        }
    }
}
