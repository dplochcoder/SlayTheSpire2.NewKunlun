using BaseLib.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.Powers;
using NewKunlun.NewKunlunCode.Cards;
using NewKunlun.NewKunlunCode.Powers;
using ParryPower = NewKunlun.NewKunlunCode.Powers.ParryPower;
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

namespace NewKunlun.NewKunlunCode.Tips;

public static class Tip
{
    public static IHoverTip TalismanDashCard(Player? player) =>
        Card<TalismanDashCard>(upgrade: Cards.TalismanDashCard.IsUpgradedAnywhere(player));

    public static IEnumerable<IHoverTip> TalismanDashCardWithTips(Player? player) =>
        CardWithTips<TalismanDashCard>(upgrade: Cards.TalismanDashCard.IsUpgradedAnywhere(player));

    public static IEnumerable<IHoverTip> Adroit() => Enchantment<Adroit>();

    public static IHoverTip AzureBow() => Card<AzureBowCard>();

    public static IHoverTip AzureSand(bool upgrade = false) => Card<AzureSandCard>(upgrade);

    public static IHoverTip AzureSandMagazine() => Power<AzureSandMagazinePower>();

    public static IHoverTip Block() => Static(StaticHoverTip.Block);

    public static IHoverTip Boost() => Static(CustomStaticHoverTip.Boost);

    public static IHoverTip CloudPiercer() => Card<CloudPiercerCard>();

    public static IHoverTip DarkSteel(bool upgrade = false) => Card<DarkSteelCard>(upgrade);

    public static IHoverTip Detonate() => Static(CustomStaticHoverTip.Detonate);

    public static IHoverTip Dexterity() => Power<DexterityPower>();

    public static IHoverTip Discharge() => Static(CustomStaticHoverTip.Discharge);

    public static IHoverTip Exhaust() => Keyword(CardKeyword.Exhaust);

    public static IHoverTip Fatal() => Static(StaticHoverTip.Fatal);

    public static IHoverTip Imperfect() => Power<ImperfectPower>();

    public static IHoverTip InternalDamage() => Power<InternalDamagePower>();

    public static IHoverTip Lock() => Static(CustomStaticHoverTip.Lock);

    public static IHoverTip Malfunction() => Card<MalfunctionCard>();

    public static IHoverTip Mark() => Static(CustomStaticHoverTip.Mark);

    public static IHoverTip Parry() => Power<ParryPower>();

    public static IHoverTip ParryCard() => Static(CustomStaticHoverTip.ParryCard);

    public static IHoverTip PreciseParry() => Static(CustomStaticHoverTip.PreciseParry);

    public static IHoverTip Purge() => Keyword(BaseLibKeywords.Purge);

    public static IHoverTip Reload() => Static(CustomStaticHoverTip.Reload);

    public static IHoverTip Talisman() => Power<TalismanPower>();

    public static IHoverTip QiCharge() => Power<QiChargePower>();

    public static IHoverTip Retain() => Keyword(CardKeyword.Retain);

    public static IHoverTip ShadowHunter() => Card<ShadowHunterCard>();

    public static IHoverTip Sharpen() => Static(CustomStaticHoverTip.Sharpen);

    public static IHoverTip Smolder() => Card<SmolderCard>();

    public static IHoverTip Speed() => Power<SpeedPower>();

    public static IHoverTip Strength() => Power<StrengthPower>();

    public static IHoverTip ThunderBuster() => Card<ThunderBusterCard>();

    public static IHoverTip Vitality() => Power<VitalityPower>();

    public static IHoverTip Void() => Card<MegaCrit.Sts2.Core.Models.Cards.Void>();

    public static IHoverTip Vulnerable() => Power<VulnerablePower>();

    public static IHoverTip Weak() => Power<WeakPower>();

    public static IHoverTip TalismanDetonateCard(Player? player) =>
        Card<TalismanDetonateCard>(upgrade: Cards.TalismanDetonateCard.IsUpgradedAnywhere(player));

    public static IEnumerable<IHoverTip> TalismanDetonateCardWithTips(Player? player) =>
        CardWithTips<TalismanDetonateCard>(
            upgrade: Cards.TalismanDetonateCard.IsUpgradedAnywhere(player)
        );

    public static IHoverTip Card<T>(bool upgrade = false)
        where T : CardModel => HoverTipFactory.FromCard<T>(upgrade);

    private static IEnumerable<IHoverTip> CardWithTips<T>(bool upgrade = false)
        where T : CardModel => HoverTipFactory.FromCardWithCardHoverTips<T>(upgrade);

    private static IEnumerable<IHoverTip> Enchantment<T>()
        where T : EnchantmentModel => HoverTipFactory.FromEnchantment<T>();

    private static IHoverTip Keyword(CardKeyword keyword) => HoverTipFactory.FromKeyword(keyword);

    private static IHoverTip Power<T>()
        where T : PowerModel => HoverTipFactory.FromPower<T>();

    private static IHoverTip Static(StaticHoverTip tip) => HoverTipFactory.Static(tip);
}
