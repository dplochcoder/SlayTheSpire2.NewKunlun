using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Shuanshuan;

public abstract class PortraitRelic : NewKunlunRelic
{
    private static readonly Dictionary<Type, Type> CharacterToRelic = [];

    private static RelicModel YiRelic => ModelDb.Relic<PortraitOfTheSolarianRelic>();

    public static void RegisterCharacter<TCharacter, TRelic>()
        where TCharacter : CharacterModel
        where TRelic : RelicModel => CharacterToRelic.Add(typeof(TCharacter), typeof(TRelic));

    public static RelicModel ForPlayer(Player player) =>
        player.Character switch
        {
            Ironclad => ModelDb.Relic<PortraitOfTheWarriorRelic>(),
            Silent => ModelDb.Relic<PortraitOfTheRogueRelic>(),
            Regent => ModelDb.Relic<PortraitOfTheMonarchRelic>(),
            Necrobinder => ModelDb.Relic<PortraitOfTheLichRelic>(),
            Character.Yi => YiRelic,
            _ => CharacterToRelic.TryGetValue(player.Character.GetType(), out var relicType)
                ? ModelDb.AllRelics.FirstOrDefault(r => r.GetType() == relicType) ?? YiRelic
                : YiRelic,
        };

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool HasUponPickupEffect => true;

    private CardReward ForRarity(CardRarity rarity)
    {
        CardReward reward = new(
            new CardCreationOptions(
                [ModelDb.CardPool<ColorlessCardPool>()],
                CardCreationSource.Other,
                CardRarityOddsType.Uniform,
                c => c.Rarity == rarity
            ).WithFlags(
                CardCreationFlags.NoRarityModification | CardCreationFlags.NoCardPoolModifications
            ),
            3,
            Owner
        );
        reward.AfterGenerated += UpgradeCards;
        return reward;

        void UpgradeCards()
        {
            foreach (
                var card in reward.Cards.Where(c => c is { IsUpgradable: true, IsUpgraded: false })
            )
                CardCmd.Upgrade(card);
        }
    }

    public override Task AfterObtained()
    {
        RewardsSet set = new(Owner);
        set.Rewards.Add(ForRarity(CardRarity.Common));
        set.Rewards.Add(ForRarity(CardRarity.Uncommon));
        set.Rewards.Add(ForRarity(CardRarity.Rare));
        return set.Offer();
    }
}

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Portrait of the Automaton",
    description: "On pickup, gain 3 upgraded Colorless card rewards.",
    flavor: ""
)]
public class PortraitOfTheAutomatonRelic : PortraitRelic { }

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Portrait of the Lich",
    description: "On pickup, gain 3 upgraded Colorless card rewards.",
    flavor: ""
)]
public class PortraitOfTheLichRelic : PortraitRelic { }

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Portrait of the Monarch",
    description: "On pickup, gain 3 upgraded Colorless card rewards.",
    flavor: ""
)]
public class PortraitOfTheMonarchRelic : PortraitRelic { }

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Portrait of the Rogue",
    description: "On pickup, gain 3 upgraded Colorless card rewards.",
    flavor: ""
)]
public class PortraitOfTheRogueRelic : PortraitRelic { }

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Portrait of the Solarian",
    description: "On pickup, gain 3 upgraded Colorless card rewards.",
    flavor: ""
)]
public class PortraitOfTheSolarianRelic : PortraitRelic { }

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Portrait of the Warrior",
    description: "On pickup, gain 3 upgraded Colorless card rewards.",
    flavor: ""
)]
public class PortraitOfTheWarriorRelic : PortraitRelic { }
