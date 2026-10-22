using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.RestSiteOptions;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(YiRelicPool))]
[RelicLocalization(
    title: "Metal Detector",
    description: "At rest sites, you can [gold]Salvage[/gold] to find {GoldMin} to {GoldMax} gold.",
    flavor: ""
)]
public partial class MetalDetectorRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public const int GoldMinValue = 120;
    public const int GoldMaxValue = 200;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new GoldVar(nameof(GoldMin), GoldMinValue), new GoldVar(nameof(GoldMax), GoldMaxValue)];

    public override bool TryModifyRestSiteOptions(
        Player player,
        ICollection<RestSiteOption> options
    )
    {
        options.Add(new MetalDetectorRestSiteOption(player));
        return true;
    }
}
