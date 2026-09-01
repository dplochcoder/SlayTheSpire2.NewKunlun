using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

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

    public static IHoverTip TalismanDetonateCard(Player? player) =>
        Card<NewKunlun.NewKunlunCode.Cards.TalismanDetonateCard>(
            upgraded: NewKunlun.NewKunlunCode.Cards.TalismanDetonateCard.IsUpgradedAnywhere(player)
        );

    public static IHoverTip Card<T>(bool upgraded = false)
        where T : CardModel => HoverTipFactory.FromCard<T>(upgraded);

    public static IHoverTip Power<T>()
        where T : PowerModel => HoverTipFactory.FromPower<T>();
}
