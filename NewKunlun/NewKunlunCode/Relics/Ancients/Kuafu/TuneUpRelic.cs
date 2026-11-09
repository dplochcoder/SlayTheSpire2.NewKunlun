using BaseLib.Common.Rewards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Kuafu;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Tune Up",
    description: "On pickup, gain 1 potion slot, 1 random potion, 1 card reward, 1 upgrade and 1 removal.\nAt the start of combat, gain {Energy:energyIcons()} and draw 1 card.",
    flavor: ""
)]
public partial class TuneUpRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

    public override bool HasUponPickupEffect => true;

    public override async Task AfterObtained()
    {
        await PlayerCmd.GainMaxPotionCount(1, Owner);

        RewardsSet set = new(Owner);
        set.Rewards.Add(
            new PotionReward(
                PotionFactory.CreateRandomPotionOutOfCombat(Owner, Owner.PlayerRng.Rewards),
                Owner
            )
        );
        set.Rewards.Add(
            new CardReward(
                new CardCreationOptions(
                    [Owner.Character.CardPool],
                    CardCreationSource.Other,
                    CardRarityOddsType.EliteEncounter
                ),
                3,
                Owner
            )
        );
        set.Rewards.Add(new CardUpgradeReward(Owner) { Amount = 1 });
        set.Rewards.Add(new CardRemovalReward(Owner));
        await set.Offer();
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (side != Owner.Creature.Side || combatState.RoundNumber != 1)
            return;

        await PlayerCmd.GainEnergy(Energy.IntValue, Owner);
    }

    public override decimal ModifyHandDraw(Player player, decimal count) =>
        player == Owner && player.Creature.CombatState?.RoundNumber is 1 ? count + 1 : count;
}
