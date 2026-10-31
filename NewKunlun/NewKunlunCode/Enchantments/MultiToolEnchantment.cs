using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Enchantments;

[EnchantmentLocalization(
    title: "Multi Tool",
    description: "On draw, add all other [gold]Multi Tool[/gold] cards from your draw pile into your hand."
)]
public class MultiToolEnchantment : NewKunlunEnchantment
{
    public override async Task AfterCardDrawn(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool fromHandDraw
    )
    {
        if (card != Card || Card.Owner.PlayerCombatState == null)
            return;

        List<CardModel> tools =
        [
            .. Card.Owner.PlayerCombatState.DrawPile.Cards.Where(c =>
                c.Enchantment is MultiToolEnchantment
            ),
        ];
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(tools, PileType.Hand), time: 0.7f);
    }
}
