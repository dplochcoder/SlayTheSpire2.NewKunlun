using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Saves.Runs;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Enchantments;

[EnchantmentLocalization(
    title: "Loyal",
    description: "Next turn, return this card to your hand.",
    extraCardText: "Next turn, return to your hand."
)]
public class LoyalEnchantment : NewKunlunEnchantment
{
    public override bool CanEnchantCardType(CardType cardType) =>
        cardType is CardType.Attack or CardType.Skill;

    public override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        PlayedLastTurn = true;
        return Task.CompletedTask;
    }

    [SavedProperty]
    public bool PlayedLastTurn
    {
        get;
        set
        {
            AssertMutable();
            field = value;
        }
    }

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState
    )
    {
        if (player != Card.Owner)
            return;

        if (PlayedLastTurn)
        {
            PlayedLastTurn = false;
            if (Card.Pile?.Type != PileType.Hand)
                await CardPileCmd.Add(Card, PileType.Hand);
        }
    }
}
