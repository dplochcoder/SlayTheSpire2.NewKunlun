using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(YiRelicPool))]
[RelicLocalization(
    title: "Scrap",
    description: "Receive {GoldMin} to {GoldMax} gold, 1 common potion, and 1 common card reward.",
    flavor: ""
)]
public partial class ScrapRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new GoldVar(nameof(GoldMin), 60), new GoldVar(nameof(GoldMax), 100)];

    public override bool HasUponPickupEffect => true;

    public override async Task AfterObtained()
    {
        RewardsSet set = new(Owner);
        var gold = Owner.PlayerRng.Rewards.NextInt(GoldMin.IntValue, GoldMax.IntValue + 1);
        set.Rewards.Add(new GoldReward(gold, Owner));
        var potion = PotionFactory.CreateRandomPotionOutOfCombat(
            Owner,
            Owner.RunState.Rng.CombatPotionGeneration
        );
        set.Rewards.Add(new PotionReward(potion, Owner));
        set.Rewards.Add(
            new CardReward(
                new CardCreationOptions(
                    [Owner.Character.CardPool],
                    CardCreationSource.Other,
                    CardRarityOddsType.Uniform,
                    c => c.Rarity == CardRarity.Common
                ),
                3,
                Owner
            )
        );

        await set.Offer();
    }
}
