using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.Powers;
using NewKunlun.NewKunlunCode.Powers;
using ParryPower = NewKunlun.NewKunlunCode.Powers.ParryPower;

public static class Tips
{
    private static HoverTip CustomKeywordHoverTip(string name)
    {
        LocString title = new("card_keywords", $"NEWKUNLUN-{name}.title");
        LocString description = new("card_keywords", $"NEWKUNLUN-{name}.description");
        return new HoverTip(title, description);
    }

    public static IHoverTip ParryCardKeyword() => CustomKeywordHoverTip("PARRY_CARD");

    public static IHoverTip TalismanDashCard(Player? player) =>
        Card<NewKunlun.NewKunlunCode.Cards.TalismanDashCard>(
            upgraded: NewKunlun.NewKunlunCode.Cards.TalismanDashCard.IsUpgradedAnywhere(player)
        );

    public static IEnumerable<IHoverTip> TalismanDashCardWithTips(Player? player) =>
        CardWithTips<NewKunlun.NewKunlunCode.Cards.TalismanDashCard>(
            upgraded: NewKunlun.NewKunlunCode.Cards.TalismanDashCard.IsUpgradedAnywhere(player)
        );

    public static IEnumerable<IHoverTip> Adroit() => Enchantment<Adroit>();

    public static IHoverTip Dexterity() => Power<DexterityPower>();

    public static IHoverTip Imperfect() => Power<ImperfectPower>();

    public static IHoverTip InternalDamage() => Power<InternalDamagePower>();

    public static IHoverTip Parry() => Power<ParryPower>();

    public static IHoverTip Talisman() => Power<TalismanPower>();

    public static IHoverTip QiCharge() => Power<QiChargePower>();

    public static IHoverTip Strength() => Power<StrengthPower>();

    public static IHoverTip Vulnerable() => Power<VulnerablePower>();

    public static IHoverTip Weak() => Power<WeakPower>();

    public static IHoverTip TalismanDetonateCard(Player? player) =>
        Card<NewKunlun.NewKunlunCode.Cards.TalismanDetonateCard>(
            upgraded: NewKunlun.NewKunlunCode.Cards.TalismanDetonateCard.IsUpgradedAnywhere(player)
        );

    public static IEnumerable<IHoverTip> TalismanDetonateCardWithTips(Player? player) =>
        CardWithTips<NewKunlun.NewKunlunCode.Cards.TalismanDetonateCard>(
            upgraded: NewKunlun.NewKunlunCode.Cards.TalismanDetonateCard.IsUpgradedAnywhere(player)
        );

    public static IHoverTip Card<T>(bool upgraded = false)
        where T : CardModel => HoverTipFactory.FromCard<T>(upgraded);

    private static IEnumerable<IHoverTip> CardWithTips<T>(bool upgraded = false)
        where T : CardModel => HoverTipFactory.FromCardWithCardHoverTips<T>(upgraded);

    private static IEnumerable<IHoverTip> Enchantment<T>()
        where T : EnchantmentModel => HoverTipFactory.FromEnchantment<T>();

    private static IHoverTip Power<T>()
        where T : PowerModel => HoverTipFactory.FromPower<T>();
}
