using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.RestSiteOptions;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "3D Printer",
    description: "At rest sites, you may spend {Gold} Gold to duplicate a card.",
    flavor: ""
)]
public partial class Printer3DRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public const int GoldCost = 90;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new GoldVar(GoldCost)];

    public override bool TryModifyRestSiteOptions(
        Player player,
        ICollection<RestSiteOption> options
    )
    {
        options.Add(new Printer3DRestSiteOption(player));
        return true;
    }
}
