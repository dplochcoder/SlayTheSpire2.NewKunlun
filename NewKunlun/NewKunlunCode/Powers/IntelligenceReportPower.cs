using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Intelligence Report",
    description: "At the end of combat, gain an extra Rare card reward."
)]
public class IntelligenceReportPower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool TryModifyRewards(Player player, List<Reward> rewards, AbstractRoom? room)
    {
        if (player.Creature != Owner)
            return false;

        rewards.Add(new CardReward(CardCreationOptions.ForRoom(player, RoomType.Boss), 3, player));
        return true;
    }
}
