using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Abacus;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Growth Serum",
    description: "After every combat, gain {MaxHp} max hp.\nYou can no longer [gold]Rest[/gold].",
    flavor: ""
)]
public partial class GrowthSerumRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(nameof(MaxHp), 2)];

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        Flash();
        await CreatureCmd.GainMaxHp(Owner.Creature, MaxHp.IntValue);
    }

    public override bool TryModifyRestSiteOptions(
        Player player,
        ICollection<RestSiteOption> options
    )
    {
        List<RestSiteOption> rest = [.. options.Where(o => o is HealRestSiteOption)];
        return rest.Aggregate(false, (current, o) => current | options.Remove(o));
    }
}
