using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Enchantments;

[EnchantmentLocalization(
    title: "Multi Tool",
    description: "When drawn, you may swap this card with any other [gold]Multi Tool[/gold] card in your draw pile.",
    customPromptA: "Choose a multi-tool component to draw."
)]
public class MultiToolEnchantment : NewKunlunEnchantment
{
    public override async Task AfterCardDrawn(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool fromHandDraw
    )
    {
        if (card != Card)
            return;

        List<CardModel> candidates =
        [
            Card,
            .. PileType
                .Deck.GetPile(Card.Owner)
                .Cards.Where(c => c.Enchantment is MultiToolEnchantment),
        ];
        var swap =
            await CardSelectCmds.FromSelectACardScreenIfPossible(
                choiceContext,
                candidates,
                Card.Owner,
                this.CustomPromptA,
                canSkip: true
            ) ?? Card;

        if (swap == Card)
            return;

        var drawPile = PileType.Draw.GetPile(Card.Owner);
        var index = drawPile.Cards.IndexOf(swap);
        await CardPileCmd.Add(card, PileType.Draw.GetPile(Card.Owner));
        await CardPileCmd.Add(swap, PileType.Hand.GetPile(Card.Owner));

        drawPile._cards.Remove(Card);
        drawPile._cards.Insert(index, Card);
    }
}
