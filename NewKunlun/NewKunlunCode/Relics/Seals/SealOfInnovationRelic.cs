using BaseLib.Common.Rewards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Seals;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Seal of Innovation",
    description: "After defeating any [gold]Elite[/gold] or [gold]Boss[/gold], you may [gold]Transform[/gold] a card.",
    flavor: ""
)]
public class SealOfInnovationRelic : NewKunlunRelic, ISealRelic<SealOfInnovationRelic>
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.Static(StaticHoverTip.Transform)];

    private static bool IsValidRoomType(RoomType roomType) =>
        roomType is RoomType.Boss or RoomType.Elite;

    public override bool TryModifyRewards(Player player, List<Reward> rewards, AbstractRoom? room)
    {
        if (
            player != Owner
            || room is not CombatRoom combatRoom
            || !IsValidRoomType(combatRoom.RoomType)
        )
            return false;

        rewards.Add(new CardTransformReward(Owner) { MaxCards = 1, Upgrade = false });
        return true;
    }
}
