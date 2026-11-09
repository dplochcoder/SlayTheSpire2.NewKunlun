using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Rhizomatic Column",
    description: "Lose 1 energy next turn.",
    smartDescription: "Lose {Amount:energyIcons()} next turn."
)]
public class RhizomaticColumnPower : NewKunlunPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player
    )
    {
        if (player.Creature != Owner)
            return;

        Flash();
        await PlayerCmd.LoseEnergy(Amount, player);
        await PowerCmd.Remove(this);
    }
}
