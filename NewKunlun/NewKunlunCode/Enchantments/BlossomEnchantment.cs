using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Enchantments;

[EnchantmentLocalization(
    title: "Blossom",
    description: "Gains [gold]Ethereal[/gold].\nNext turn, add a copy of this card to your hand.",
    extraCardText: "Next turn, add a copy of this card to your hand."
)]
public class BlossomEnchantment : NewKunlunEnchantment
{
    public override bool CanEnchantCardType(CardType cardType) =>
        cardType is CardType.Attack or CardType.Skill;

    public override bool CanEnchant(CardModel card) =>
        base.CanEnchant(card) && !card.Keywords.Contains(CardKeyword.Exhaust);

    protected override void OnEnchant() => Card.AddKeyword(CardKeyword.Ethereal);

    [SavedProperty]
    public int PlaysThisTurn
    {
        get;
        set
        {
            AssertMutable();
            field = value;
        }
    }

    protected override void AfterCloned() => PlaysThisTurn = 0;

    public override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        ++PlaysThisTurn;
        return Task.CompletedTask;
    }

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState
    )
    {
        if (player != Card.Owner)
            return;

        var plays = PlaysThisTurn;
        PlaysThisTurn = 0;
        for (var i = 0; i < plays; i++)
        {
            var clone = Card.CreateClone();
            clone.AddKeyword(CardKeyword.Ethereal);
            await CardPileCmd.Add(
                clone,
                PileType.Hand.GetPile(player),
                CardPilePosition.Top,
                clonedBy: this
            );
        }
    }
}
