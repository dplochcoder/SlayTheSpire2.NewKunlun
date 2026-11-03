using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Enchantments;

[EnchantmentLocalization(title: "Calligraphic", description: "Next turn, draw {Amount} cards.")]
public class CalligraphicEnchantment : NewKunlunEnchantment
{
    public override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay) =>
        PowerCmd.Apply<DrawCardsNextTurnPower>(
            choiceContext,
            Card.Owner.Creature,
            Amount,
            Card.Owner.Creature,
            cardPlay?.Card
        );
}
