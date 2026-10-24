using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(YiRelicPool))]
[RelicLocalization(
    title: "Computing Unit",
    description: "On pickup, gain {Rewards} Uncommon Power card rewards.\nOn your third turn of combat, lose {Energy:energyIcons()}.",
    flavor: ""
)]
public partial class ComputingUnitRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar(nameof(Rewards), 2), new EnergyVar(1)];

    public override bool HasUponPickupEffect => true;

    public override async Task AfterObtained()
    {
        RewardsSet set = new(Owner);
        for (var i = 0; i < Rewards.IntValue; i++)
            set.Rewards.Add(
                new CardReward(
                    new CardCreationOptions(
                        [Owner.Character.CardPool],
                        CardCreationSource.Other,
                        CardRarityOddsType.Uniform,
                        c => c is { Type: CardType.Power, Rarity: CardRarity.Uncommon }
                    ),
                    3,
                    Owner
                )
            );

        await set.Offer();
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (side != Owner.Creature.Side || Owner.PlayerCombatState!.TurnNumber != 3)
            return;

        Flash();
        await PlayerCmd.LoseEnergy(StackCount, Owner);
    }
}
