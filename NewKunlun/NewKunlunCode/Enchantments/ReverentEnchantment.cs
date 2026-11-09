using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Enchantments;

[EnchantmentLocalization(
    title: "Reverent",
    description: "Gain {Amount:starIcons()}.",
    extraCardText: "Gain {Amount:starIcons()}."
)]
public class ReverentEnchantment : NewKunlunEnchantment
{
    public override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay) =>
        PlayerCmd.GainStars(Amount, Card.Owner);
}
