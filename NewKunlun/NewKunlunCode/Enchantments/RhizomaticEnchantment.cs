using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Enchantments;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Enchantments;

[EnchantmentLocalization(
    title: "Rhizomatic",
    description: "The first time played this combat, regain the energy spent.",
    extraCardText: "[gold]Rhizomatic.[/gold]"
)]
public class RhizomaticEnchantment : NewKunlunEnchantment
{
    public override bool CanEnchant(CardModel card) =>
        base.CanEnchant(card) && (card.EnergyCost.CostsX || card.EnergyCost.Canonical > 0);

    public override bool HasExtraCardText => true;

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        if (
            cardPlay == null
            || cardPlay.IsAutoPlay
            || cardPlay.PlayIndex != cardPlay.PlayCount - 1
            || cardPlay.Resources.EnergySpent <= 0
        )
            return;

        await PlayerCmd.GainEnergy(cardPlay.Resources.EnergySpent, Card.Owner);
        Status = EnchantmentStatus.Disabled;
    }
}
