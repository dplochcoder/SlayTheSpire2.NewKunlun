using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Lear;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Celestial Passkey",
    description: "Gain 2 Rare card rewards.",
    flavor: "",
    selectionScreenPrompt: "Choose a card to add to your deck."
)]
public class CelestialPasskeyRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool HasUponPickupEffect => true;

    public override Task AfterObtained()
    {
        RewardsSet set = new(Owner);
        for (var i = 0; i < 2; i++)
            set.Rewards.Add(
                new CardReward(
                    new(
                        [Owner.Character.CardPool],
                        CardCreationSource.Other,
                        CardRarityOddsType.Uniform,
                        c => c.Rarity == CardRarity.Rare
                    ),
                    3,
                    Owner
                )
            );
        return set.Offer();
    }
}
