using BaseLib.Common.Rewards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(YiRelicPool))]
[RelicLocalization(
    title: "Seal of Innovation",
    description: "Whenever you defeat an [gold]Elite[/gold] or [gold]Boss[/gold], transform a card.",
    flavor: ""
)]
public class SealOfInnovationRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool TryModifyRewards(Player player, List<Reward> rewards, AbstractRoom? room)
    {
        if (
            player != this.Owner
            || room == null
            || (room.RoomType != RoomType.Boss && room.RoomType != RoomType.Elite)
        )
            return false;

        rewards.Add(new CardTransformReward(player) { Upgrade = false, MaxCards = 1 });
        return true;
    }
}
