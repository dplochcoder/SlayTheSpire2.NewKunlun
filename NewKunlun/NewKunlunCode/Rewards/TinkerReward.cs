using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Saves.Runs;
using NewKunlun.NewKunlunCode.Collections;
using NewKunlun.NewKunlunCode.Extensions;

namespace NewKunlun.NewKunlunCode.Rewards;

public class TinkerReward : CustomReward
{
    [CustomEnum]
    public static RewardType TinkerRewardType;

    private readonly CardRarity _rarity;

    private static readonly IReadOnlyDictionary<CardRarity, CardRarity> NextRarity = new Dictionary<
        CardRarity,
        CardRarity
    >
    {
        { CardRarity.Basic, CardRarity.Common },
        { CardRarity.Common, CardRarity.Uncommon },
        { CardRarity.Uncommon, CardRarity.Rare },
    };

    public TinkerReward(Player player, CardRarity rarity)
        : base(player) => _rarity = rarity;

    protected override string? IconPath =>
        NextRarity[_rarity] switch
        {
            CardRarity.Common => CardReward.RewardIcon,
            CardRarity.Uncommon => CardReward.UncommonRewardIcon,
            CardRarity.Rare => CardReward.RareRewardIcon,
            _ => CardReward.RewardIcon,
        };

    public override void Populate() { }

    public static async Task<bool> Tinker(Player player, CardRarity rarity)
    {
        List<CardModel> removedCards =
        [
            .. await CardSelectCmd.FromDeckForRemoval(
                player,
                new(CardSelectorPrefs.RemoveSelectionPrompt, 2)
                {
                    Cancelable = true,
                    RequireManualConfirmation = true,
                },
                c => c.Rarity == rarity && c.IsRemovable
            ),
        ];
        if (removedCards.Count != 2)
            return false;

        await CardPileCmd.RemoveFromDeck(removedCards);

        List<CardModel> newCards =
        [
            .. player
                .Character.CardPool.CardsUnlockedFor(player)
                .Where(c => c.Rarity == NextRarity[rarity]),
        ];
        var newCard = player.PlayerRng.Rewards.NextItem(newCards)!;
        newCard = player.RunState.CreateCard(newCard, player);
        if (newCard.IsUpgradable)
        {
            var src = removedCards.Count(c => c.IsUpgraded);
            if (src == 2 || src == 1 && player.PlayerRng.Rewards.NextBool())
                CardCmd.Upgrade(newCard, CardPreviewStyle.None);
        }

        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(newCard, PileType.Deck));
        return true;
    }

    protected override Task<bool> OnSelect() => Tinker(Player, _rarity);

    public override void MarkContentAsSeen() { }

    protected override RewardType RewardType => TinkerRewardType;
    public override LocString Description => null;
    public override bool IsPopulated => true;
    public override CreateRewardFromSave<CustomReward> DeserializeMethod => Deserialize;

    private static readonly IReadOnlyBimap<CardRarity, int> CodeMap =
    [
        new(CardRarity.Basic, 1),
        new(CardRarity.Common, 2),
        new(CardRarity.Uncommon, 3),
    ];

    public override SerializableReward ToSerializable() =>
        new() { RewardType = TinkerRewardType, GoldAmount = CodeMap[_rarity] };

    private static TinkerReward Deserialize(SerializableReward save, Player player) =>
        new(player, CodeMap.Inverse[save.GoldAmount]);
}
