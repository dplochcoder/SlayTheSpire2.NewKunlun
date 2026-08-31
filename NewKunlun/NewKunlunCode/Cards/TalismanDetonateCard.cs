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
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Talisman Detonate",
    description: "Spend up to {QiCharge:diff()} [gold]Qi Charges[/gold] to inflict {Damage:diff()} unblockable damage per charge, and {Vulnerable:diff()} [gold]Vulnerable[/gold], to each enemy afflicted with [gold]Talisman[/gold]."
)]
public partial class TalismanDetonateCard()
    : NewKunlunCard(2, CardType.Skill, CardRarity.Basic, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new QiChargeVar(2M),
            new DynamicVar(nameof(Vulnerable), 1M),
            new DamageVar(15M, ValueProp.Unblockable | ValueProp.Unpowered),
        ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Ethereal, CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<QiChargePower>(),
            HoverTipFactory.FromPower<VulnerablePower>(),
            HoverTipFactory.FromPower<TalismanPower>(),
        ];

    protected override bool ShouldGlowGoldInternal =>
        Owner.Creature.GetPowerAmount<QiChargePower>() > 0
        && (
            CombatState?.Enemies.Any(e =>
                e.IsHittable
                && e.GetPowerInstances<TalismanPower>().Any(p => p.Applier == Owner.Creature)
            )
            ?? false
        );

    protected override void OnUpgrade()
    {
        Vulnerable.UpgradeValueTo(2M);
        QiCharge.UpgradeValueTo(3M);
        Damage.UpgradeValueTo(19M);
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

        var charges = await QiChargeCmd.ConsumeQiCharges(
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

        await CreatureCmd.Damage(
            choiceContext,
            eligibleCreatures,
            new DamageVar(charges * Damage.BaseValue, Damage.Props),
            Owner.Creature
        );
        return;

        async Task ClearPowers()
        {
            foreach (
                var power in CombatState?.Enemies.SelectMany(e =>
                    e.GetPowerInstances<TalismanPower>().Where(p => p.Applier == player)
                )
                    ?? []
            )
                await PowerCmd.Remove(power);
        }
    }
}
