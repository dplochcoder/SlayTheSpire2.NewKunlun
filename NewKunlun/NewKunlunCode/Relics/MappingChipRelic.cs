using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.RestSiteOptions;

namespace NewKunlun.NewKunlunCode.Relics;

[RelicLocalization(
    title: "Mapping Chip",
    description: "You can ignore pathing restrictions {Uses} {Uses:plural:time|times}.\nAt rest sites, you can spend {Cost} gold to do it two more times.",
    flavor: ""
)]
public partial class MappingChipRelic : NewKunlunRelic
{
    public const int GOLD_COST = 50;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar(nameof(Uses), 2), new GoldVar(nameof(Cost), GOLD_COST)];

    public int UsesRemaining
    {
        get => Uses.IntValue;
        set
        {
            AssertMutable();
            if (Uses.IntValue != value)
                Flash();

            Uses.BaseValue = value;
            Status = IsUsedUp ? RelicStatus.Disabled : RelicStatus.Normal;
            InvokeDisplayAmountChanged();
        }
    }

    public override bool ShowCounter => UsesRemaining > 0;

    public override int DisplayAmount => UsesRemaining;

    public override bool IsUsedUp => UsesRemaining == 0;

    public override bool IsAllowed(IRunState runState) => runState.Players.Count == 1;

    public override bool ShouldAllowFreeTravel() => !IsUsedUp;

    private static bool UsedFreeTravel(RunState runState)
    {
        var coords = runState.VisitedMapCoords;
        var prevCoord = coords[^2];
        var prevPoint = runState.Map.GetPoint(prevCoord);
        if (prevPoint == null)
            return false;

        var curPoint = runState.CurrentMapPoint;
        return curPoint != null && !prevPoint.Children.Contains(curPoint);
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (
            !this.IsNextUnused
            || Owner.RunState.CurrentRoomCount > 1
            || Owner.RunState is not RunState runState
            || runState.VisitedMapCoords.Count <= 1
        )
            return Task.CompletedTask;
        if (UsedFreeTravel(runState))
            --UsesRemaining;
        return Task.CompletedTask;
    }

    public override bool TryModifyRestSiteOptions(
        Player player,
        ICollection<RestSiteOption> options
    )
    {
        options.Add(new MappingChipRestSiteOption(player));
        return true;
    }
}
