using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Shanhai Coupon",
    description: "Your next shop purchase is free.",
    flavor: ""
)]
public class ShanhaiRebateRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override bool IsAllowedInShops => false;

    [SavedProperty]
    public bool CouponUsed
    {
        get => field;
        set
        {
            AssertMutable();
            field = value;
            if (IsUsedUp)
                Status = RelicStatus.Disabled;
        }
    }

    public override bool IsUsedUp => !CouponUsed;

    public override decimal ModifyMerchantPrice(
        Player player,
        MerchantEntry entry,
        decimal originalPrice
    ) => (player == Owner && !IsUsedUp) ? 0 : originalPrice;

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (!IsUsedUp)
            Status =
                room is MerchantRoom && this.IsNextUnused ? RelicStatus.Active : RelicStatus.Normal;

        return Task.CompletedTask;
    }

    private MerchantEntry? _alreadyProcessed;

    public override Task AfterItemPurchased(
        Player player,
        MerchantEntry itemPurchased,
        int goldSpent
    )
    {
        if (player != Owner || !this.IsNextUnused || _alreadyProcessed == itemPurchased)
            return Task.CompletedTask;

        _alreadyProcessed = itemPurchased;
        Flash();
        CouponUsed = true;
        this.NextUnused?.Status = RelicStatus.Active;
        return Task.CompletedTask;
    }
}
