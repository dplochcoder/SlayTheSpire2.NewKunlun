using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Enchantments;

[EnchantmentLocalization(
    title: "Amortized",
    description: "Regain the energy spent playing this card next turn."
)]
public class AmortizedEnchantment : NewKunlunEnchantment
{
    public override bool HasExtraCardText => true;

    public override bool CanEnchant(CardModel card) =>
        base.CanEnchant(card) && (card.EnergyCost.CostsX || card.EnergyCost.Canonical > 0);

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        if (
            cardPlay == null
            || cardPlay.IsAutoPlay
            || cardPlay.PlayIndex != cardPlay.PlayCount - 1
            || cardPlay.Resources.EnergySpent <= 0
        )
            return;

        await PowerCmd.Apply<EnergyNextTurnPower>(
            choiceContext,
            Card.Owner.Creature,
            cardPlay.Resources.EnergySpent,
            Card.Owner.Creature,
            cardPlay.Card
        );
    }
}
