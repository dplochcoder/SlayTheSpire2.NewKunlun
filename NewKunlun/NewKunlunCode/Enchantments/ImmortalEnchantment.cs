using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using NewKunlun.NewKunlunCode.Cards.Statuses;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Enchantments;

[EnchantmentLocalization(
    title: "Immortal",
    description: "Add a copy of this card and two [gold]Weary[/gold] to your discard pile."
)]
public class ImmortalEnchantment : NewKunlunEnchantment
{
    public override bool CanEnchantCardType(CardType cardType) => cardType == CardType.Power;

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        if (Card.Owner.Creature.CombatState is not { } combatState)
            return;

        CardCmd.PreviewCardPileAdd(
            await CardPileCmd.AddGeneratedCardsToCombat(
                [
                    Card.CreateClone(),
                    combatState.CreateCard<WearyCard>(Card.Owner),
                    combatState.CreateCard<WearyCard>(Card.Owner),
                ],
                PileType.Discard,
                Card.Owner
            )
        );
    }
}
