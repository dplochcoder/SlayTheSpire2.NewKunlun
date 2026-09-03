using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.Powers;
using NewKunlun.NewKunlunCode.Cards;
using NewKunlun.NewKunlunCode.Powers;
using ParryPower = NewKunlun.NewKunlunCode.Powers.ParryPower;

namespace NewKunlun.NewKunlunCode.Tips;

public static class Tip
{
    private static HoverTip CustomKeywordHoverTip(string name)
    {
        LocString title = new("card_keywords", $"NEWKUNLUN-{name}.title");
        LocString description = new("card_keywords", $"NEWKUNLUN-{name}.description");
        return new HoverTip(title, description);
    }

    public static IHoverTip ParryCardKeyword() => CustomKeywordHoverTip("PARRY_CARD");

    public static IHoverTip TalismanDashCard(Player? player) =>
        Card<TalismanDashCard>(upgraded: Cards.TalismanDashCard.IsUpgradedAnywhere(player));

    public static IEnumerable<IHoverTip> TalismanDashCardWithTips(Player? player) =>
        CardWithTips<TalismanDashCard>(upgrade: Cards.TalismanDashCard.IsUpgradedAnywhere(player));

    public static IEnumerable<IHoverTip> Adroit() => Enchantment<Adroit>();

    public static IHoverTip AzureSandCard(bool upgrade = false) => Card<AzureSandCard>(upgrade);

    public static IHoverTip AzureSandPower() => Power<AzureSandPower>();

    public static IHoverTip CloudPiercerCard() => Card<CloudPiercerCard>();

    public static IHoverTip DarkSteelCard(bool upgraded = false) => Card<DarkSteelCard>(upgraded);

    public static IHoverTip DarkSteelPower() => Power<DarkSteelPower>();

    public static IHoverTip Dexterity() => Power<DexterityPower>();

    public static IHoverTip Exhaust() => Keyword(CardKeyword.Exhaust);

    public static IHoverTip Imperfect() => Power<ImperfectPower>();

    public static IHoverTip InternalDamage() => Power<InternalDamagePower>();

    public static IHoverTip Malfunction() => Card<MalfunctionCard>();

    public static IHoverTip Parry() => Power<ParryPower>();

    public static IHoverTip Talisman() => Power<TalismanPower>();

    public static IHoverTip QiCharge() => Power<QiChargePower>();

    public static IHoverTip Retain() => Keyword(CardKeyword.Retain);

    public static IHoverTip ShadowHunterCard() => Card<ShadowHunterCard>();

    public static IHoverTip Smolder() => Card<SmolderCard>();

    public static IHoverTip Strength() => Power<StrengthPower>();

    public static IHoverTip ThunderBusterCard() => Card<ThunderBusterCard>();

    public static IHoverTip Void() => Card<MegaCrit.Sts2.Core.Models.Cards.Void>();

    public static IHoverTip Vulnerable() => Power<VulnerablePower>();

    public static IHoverTip Weak() => Power<WeakPower>();

    public static IHoverTip TalismanDetonateCard(Player? player) =>
        Card<TalismanDetonateCard>(upgraded: Cards.TalismanDetonateCard.IsUpgradedAnywhere(player));

    public static IEnumerable<IHoverTip> TalismanDetonateCardWithTips(Player? player) =>
        CardWithTips<TalismanDetonateCard>(
            upgrade: Cards.TalismanDetonateCard.IsUpgradedAnywhere(player)
        );

    public static IHoverTip Card<T>(bool upgraded = false)
        where T : CardModel => HoverTipFactory.FromCard<T>(upgraded);

    private static IEnumerable<IHoverTip> CardWithTips<T>(bool upgrade = false)
        where T : CardModel => HoverTipFactory.FromCardWithCardHoverTips<T>(upgrade);

    private static IEnumerable<IHoverTip> Enchantment<T>()
        where T : EnchantmentModel => HoverTipFactory.FromEnchantment<T>();

    private static IHoverTip Keyword(CardKeyword keyword) => HoverTipFactory.FromKeyword(keyword);

    private static IHoverTip Power<T>()
        where T : PowerModel => HoverTipFactory.FromPower<T>();
}
