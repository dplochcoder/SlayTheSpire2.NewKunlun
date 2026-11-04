using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Abacus;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Jie Hammer",
    description: "The next {UsesRemaining} {UsesRemaining:plural:time|times} you [gold]Smith[/gold], upgrade an additional card.",
    flavor: ""
)]
public partial class JieHammerRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new(nameof(UsesRemaining), 3)];

    public override bool ShowCounter => true;

    public override int DisplayAmount => UsesRemaining.IntValue;

    public override bool IsUsedUp => UsesRemaining.IntValue <= 0;

    public bool SmithProcessed
    {
        get;
        set
        {
            AssertMutable();
            field = value;
        }
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        SmithProcessed = false;
        if (!IsUsedUp)
            Status =
                room is RestSiteRoom && this.IsNextUnused ? RelicStatus.Active : RelicStatus.Normal;
        return Task.CompletedTask;
    }

    public override bool TryModifyRestSiteOptions(
        Player player,
        ICollection<RestSiteOption> options
    )
    {
        if (player != Owner && !this.IsNextUnused)
            return false;

        var smith = options.OfType<SmithRestSiteOption>().FirstOrDefault();
        if (smith == null)
            return false;

        ++smith.SmithCount;
        return true;
    }

    public override Task AfterRestSiteSmith(Player player)
    {
        if (player != Owner || !this.IsNextUnused || SmithProcessed)
            return Task.CompletedTask;

        SmithProcessed = true;
        --UsesRemaining.BaseValue;
        InvokeDisplayAmountChanged();
        Status = IsUsedUp ? RelicStatus.Disabled : RelicStatus.Normal;
        return Task.CompletedTask;
    }
}
