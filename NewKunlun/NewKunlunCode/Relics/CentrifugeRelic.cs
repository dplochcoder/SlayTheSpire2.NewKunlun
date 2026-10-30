using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Runs;
using NewKunlun.NewKunlunCode.Enchantments;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Centrifuge",
    description: "Receive [gold]Poor Sleep[/gold].\nYour next {Uses} card {Uses:plural:reward gains|rewards gain} random enchantments.",
    flavor: ""
)]
public partial class CentrifugeRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new(nameof(Uses), 3)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<PoorSleep>()];

    private int UsesRemaining
    {
        get => Uses.IntValue;
        set
        {
            AssertMutable();
            Uses.BaseValue = value;
            Status = IsUsedUp ? RelicStatus.Disabled : RelicStatus.Normal;
            InvokeDisplayAmountChanged();
        }
    }

    public override bool ShowCounter => UsesRemaining > 0;

    public override int DisplayAmount => UsesRemaining;

    public override bool IsUsedUp => UsesRemaining == 0;

    private const EnchantmentOption.Rarity Common = EnchantmentOption.Rarity.Common;
    private const EnchantmentOption.Rarity Uncommon = EnchantmentOption.Rarity.Uncommon;
    private const EnchantmentOption.Rarity Rare = EnchantmentOption.Rarity.Rare;

    private record EnchantmentOption
    {
        public enum Rarity
        {
            Common,
            Uncommon,
            Rare,
        }

        public required Rarity OptionRarity;
        public required EnchantmentModel Model;
        public required IReadOnlyList<int> Amounts;
        public required Func<CardModel, bool> Filter;
    }

    private static EnchantmentOption Option<T>(
        EnchantmentOption.Rarity rarity,
        int amount = 1,
        Func<CardModel, bool>? filter = null
    )
        where T : EnchantmentModel =>
        new()
        {
            OptionRarity = rarity,
            Model = ModelDb.Enchantment<T>(),
            Amounts = [amount],
            Filter = filter ?? (_ => true),
        };

    private static EnchantmentOption Option<T>(
        EnchantmentOption.Rarity rarity,
        IReadOnlyList<int> amounts,
        Func<CardModel, bool>? filter = null
    )
        where T : EnchantmentModel =>
        new()
        {
            OptionRarity = rarity,
            Model = ModelDb.Enchantment<T>(),
            Amounts = [.. amounts],
            Filter = filter ?? (_ => true),
        };

    private static WeightedList<EnchantmentOption.Rarity> Weights
    {
        get
        {
            WeightedList<EnchantmentOption.Rarity> weights = [];
            weights.Add(Common, 5);
            weights.Add(Uncommon, 3);
            weights.Add(Rare, 1);
            return weights;
        }
    }

    private static List<EnchantmentOption> AllOptions =>
        [
            Option<Adroit>(Uncommon, [2, 3, 3, 4]),
            Option<AmortizedEnchantment>(Rare),
            Option<AntiquatedEnchantment>(Uncommon),
            Option<Corrupted>(Uncommon),
            Option<Glam>(Rare),
            Option<Inky>(Uncommon, 1, c => c.Type == CardType.Attack),
            Option<Instinct>(Rare),
            Option<Momentum>(Uncommon, [4, 5]),
            Option<Nimble>(Common, [2, 2, 3]),
            Option<PerfectFit>(Uncommon),
            Option<PoiseEnchantment>(Rare),
            Option<PreciseEnchantment>(Rare),
            Option<RhizomaticEnchantment>(Rare),
            Option<RoyallyApproved>(Rare),
            Option<Sharp>(Common, [3]),
            Option<Slither>(Uncommon),
            Option<SoulsPower>(Rare),
            Option<Sown>(Uncommon),
            Option<Steady>(Uncommon),
            Option<Swift>(Common, [1, 1, 2]),
            Option<VenomousEnchantment>(Uncommon, [3, 3, 4]),
            Option<Vigorous>(Common, [7, 8, 9]),
        ];

    private bool SelectEnchantment(CardModel card, out EnchantmentOption option)
    {
        var weights = Weights;
        while (weights.Count > 0)
        {
            var rarity = weights.GetRandom(Owner.PlayerRng.Rewards);
            weights.Remove(rarity);

            List<EnchantmentOption> options =
            [
                .. AllOptions.Where(o =>
                    o.OptionRarity == rarity && o.Filter(card) && o.Model.CanEnchant(card)
                ),
            ];
            if (options.Count == 0)
                continue;

            option = Owner.PlayerRng.Rewards.NextItem(options)!;
            return true;
        }

        option = AllOptions[0];
        return false;
    }

    public override bool TryModifyCardRewardOptionsLate(
        Player player,
        List<CardCreationResult> cardRewards,
        CardCreationOptions options
    )
    {
        if (
            player != Owner
            || !options.Flags.HasFlag(CardCreationFlags.IsCardReward)
            || !this.IsNextUnused
        )
            return false;

        foreach (var cardReward in cardRewards)
        {
            if (!SelectEnchantment(cardReward.Card, out var option))
                continue;

            var clone = Owner.RunState.CloneCard(cardReward.Card);
            List<int> amounts = [.. option.Amounts];
            Owner.PlayerRng.Rewards.Shuffle(amounts);
            CardCmd.Enchant(option.Model, clone, amounts[0]);
        }
        return true;
    }

    public override Task AfterModifyingCardRewardOptions()
    {
        if (--StackCount <= 0)
            Status = RelicStatus.Disabled;

        return Task.CompletedTask;
    }
}
