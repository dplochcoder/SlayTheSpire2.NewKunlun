using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.RestSiteOptions;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Kuafu;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "3D Printer",
    description: "At rest sites, you duplicate any Common, Uncommon, or Rare card in your deck for {CommonGold}, {UncommonGold}, or {RareGold} respectively.",
    flavor: ""
)]
public partial class Printer3DRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public static bool TryGetCost(CardRarity rarity, out int cost)
    {
        switch (rarity)
        {
            case CardRarity.Common:
                cost = 30;
                return true;
            case CardRarity.Uncommon:
                cost = 60;
                return true;
            case CardRarity.Rare:
                cost = 120;
                return true;
            default:
                cost = 0;
                return false;
        }
    }

    public static int GetCost(CardRarity rarity) =>
        !TryGetCost(rarity, out var cost)
            ? throw new ArgumentException($"{nameof(rarity)}: {rarity}")
            : cost;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new GoldVar(nameof(CommonGold), GetCost(CardRarity.Common)),
            new GoldVar(nameof(UncommonGold), GetCost(CardRarity.Uncommon)),
            new GoldVar(nameof(RareGold), GetCost(CardRarity.Rare)),
        ];

    public override bool TryModifyRestSiteOptions(
        Player player,
        ICollection<RestSiteOption> options
    )
    {
        options.Add(new Printer3DRestSiteOption(player));
        return true;
    }
}
