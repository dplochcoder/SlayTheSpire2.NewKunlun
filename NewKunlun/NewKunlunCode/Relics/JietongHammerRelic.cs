using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Jietong Hammer",
    description: "The next {Uses} {Uses:plural:time|times} you [gold]Smith[/gold], upgrade an additional card.",
    flavor: ""
)]
public partial class JietongHammerRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new(nameof(Uses), 2)];

    private int UsesRemaining
    {
        get => Uses.IntValue;
        set
        {
            AssertMutable();
            Uses.BaseValue = value;
            Status = IsUsedUp ? RelicStatus.Disabled : RelicStatus.Normal;
            InvokeDisplayAmountChanged();
        }
    }

    public override bool ShowCounter => UsesRemaining > 0;

    public override int DisplayAmount => UsesRemaining;

    public override bool IsUsedUp => UsesRemaining == 0;

    private bool _smithProcessed;

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        _smithProcessed = false;
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
        if (player != Owner || !this.IsNextUnused || _smithProcessed)
            return Task.CompletedTask;

        _smithProcessed = true;
        --UsesRemaining;
        return Task.CompletedTask;
    }
}
