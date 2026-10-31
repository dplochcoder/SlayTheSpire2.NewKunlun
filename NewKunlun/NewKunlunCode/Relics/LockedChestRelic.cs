using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves.Runs;
using NewKunlun.NewKunlunCode.Cards;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using HackCard = NewKunlun.NewKunlunCode.Cards.Ancients.HackCard;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Locked Chest",
    description: "On pickup, add 1 [gold]Hack[/gold] to your deck.",
    flavor: ""
)]
public class LockedChestRelic : NewKunlunRelic
{
    public enum State
    {
        Closed,
        Opened,
        Looted,
    }

    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<HackCard>()];

    [SavedProperty]
    public State CurrentState
    {
        get;
        set
        {
            AssertMutable();
            field = value;
            UpdateStatus();
        }
    }

    private void UpdateStatus()
    {
        Status = CurrentState switch
        {
            State.Closed => RelicStatus.Normal,
            State.Opened => RelicStatus.Active,
            _ => RelicStatus.Disabled,
        };
        // TODO: Visuals
    }

    public override bool HasUponPickupEffect => true;

    public override Task AfterObtained() => Owner.AddCardToDeck<HackCard>();
}
