using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using NewKunlun.NewKunlunCode.Cards;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Rhythm Chop",
    description: "Whenever you have 3 or more [gold]Qi Charges[/gold] at the start of your turn, put [gold]Talisman Dash[/gold] into your hand."
)]
public class RhythmChopPower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState
    )
    {
        if (player.Creature.GetPowerAmount<QiChargePower>() < 3)
            return;

        var card = player.FindCard<TalismanDashCard>([
            PileType.Discard,
            PileType.Draw,
            PileType.Hand,
        ]);
        if (card != null)
            await CardPileCmd.Add(card, PileType.Hand.GetPile(player), CardPilePosition.Top);
    }
}
