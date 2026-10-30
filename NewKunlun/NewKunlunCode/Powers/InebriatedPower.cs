using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Cards;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Inebriated",
    description: "Whenever you play a card, randomize the costs of all cards in your hand."
)]
public class InebriatedPower : NewKunlunPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = cardPlay.Card.Owner;
        if (player.Creature != Owner)
            return Task.CompletedTask;

        foreach (var card in player.PlayerCombatState?.Hand.Cards ?? [])
        {
            if (card.EnergyCost.Canonical < 0)
                continue;
            card.EnergyCost.SetThisCombat(player.RunState.Rng.CombatEnergyCosts.NextInt(4));
            NCard.FindOnTable(card)?.PlayRandomizeCostAnim();
        }
        return Task.CompletedTask;
    }
}
