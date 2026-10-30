using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Gutwrench Fruit",
    description: "On pickup, gain {PotionSlots} potion slots and {Potions} random Potions.\nWhenever you use a potion, gain {MaxHp} max hp and heal {Heal} additional hp.",
    flavor: ""
)]
public partial class GutwrenchFruitRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new(nameof(PotionSlots), 2),
            new(nameof(Potions), 2),
            new HealVar(nameof(MaxHp), 3),
            new HealVar(nameof(Heal), 4),
        ];

    public override bool HasUponPickupEffect => true;

    public override async Task AfterObtained()
    {
        await PlayerCmd.GainMaxPotionCount(PotionSlots.IntValue, Owner);
        for (var i = 0; i < Potions.IntValue; i++)
            await PotionCmd.TryToProcure(
                PotionFactory.CreateRandomPotionOutOfCombat(
                    Owner,
                    Owner.RunState.Rng.CombatPotionGeneration
                ),
                Owner
            );
    }

    public override async Task AfterPotionUsed(PotionModel potion, Creature? target)
    {
        if (target != Owner.Creature)
            return;

        Flash();
        await CreatureCmd.GainMaxHp(target, MaxHp.IntValue);
        await CreatureCmd.Heal(target, Heal.IntValue);
    }
}
