using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Random;
using NewKunlun.NewKunlunCode.Enchantments;

namespace NewKunlun.NewKunlunCode.Commands;

public static class RandomEnchantCmd
{
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
        public required Func<Player, CardModel, bool> Filter;
    }

    private static EnchantmentOption Option<T>(
        EnchantmentOption.Rarity rarity,
        int amount = 1,
        Func<Player, CardModel, bool>? filter = null
    )
        where T : EnchantmentModel =>
        new()
        {
            OptionRarity = rarity,
            Model = ModelDb.Enchantment<T>(),
            Amounts = [amount],
            Filter = filter ?? ((_, _) => true),
        };

    private static EnchantmentOption Option<T>(
        EnchantmentOption.Rarity rarity,
        IReadOnlyList<int> amounts,
        Func<Player, CardModel, bool>? filter = null
    )
        where T : EnchantmentModel =>
        new()
        {
            OptionRarity = rarity,
            Model = ModelDb.Enchantment<T>(),
            Amounts = [.. amounts],
            Filter = filter ?? ((_, _) => true),
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
            Option<Inky>(Uncommon, 1, (_, c) => c.Type == CardType.Attack),
            Option<Instinct>(Rare),
            Option<LimitlessEnchantment>(Rare),
            Option<Momentum>(Uncommon, [4, 5]),
            Option<Nimble>(Common, [2, 2, 3]),
            Option<PerfectFit>(Uncommon),
            Option<PoiseEnchantment>(Rare),
            Option<PreciseEnchantment>(Rare),
            Option<ReverentEnchantment>(Uncommon, [1, 1, 2], (p, _) => p.Character is Regent),
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

    private static bool SelectEnchantment(
        Player player,
        Rng rng,
        CardModel card,
        out EnchantmentOption option
    )
    {
        if (card.Enchantment != null)
        {
            option = AllOptions[0];
            return false;
        }

        var weights = Weights;
        while (weights.Count > 0)
        {
            var rarity = weights.GetRandom(rng);
            weights.Remove(rarity);

            List<EnchantmentOption> options =
            [
                .. AllOptions.Where(o =>
                    o.OptionRarity == rarity && o.Filter(player, card) && o.Model.CanEnchant(card)
                ),
            ];
            if (options.Count == 0)
                continue;

            option = rng.NextItem(options)!;
            return true;
        }

        option = AllOptions[0];
        return false;
    }

    public static bool ApplyEnchantment(Player player, Rng rng, CardModel cardModel)
    {
        if (!SelectEnchantment(player, rng, cardModel, out var option))
            return false;

        CardCmd.Enchant(option.Model, cardModel, rng.NextItem(option.Amounts));
        return true;
    }
}
