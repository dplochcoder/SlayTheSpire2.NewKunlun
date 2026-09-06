using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Cards;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Powers;

namespace NewKunlun.NewKunlunCode.Commands;

public static class ReloadCmd
{
    public static async Task Reload(
        PlayerChoiceContext choiceContext,
        Player player,
        int count,
        CardModel cardModel
    )
    {
        await PowerCmd.Apply<AzureSandMagazinePower>(
            choiceContext,
            player.Creature,
            count,
            player.Creature,
            cardModel
        );

        var bow = player.FindCard<AzureBowCard>([PileType.Draw, PileType.Hand, PileType.Discard]);
        if (bow == null)
            await player.AddGeneratedCardToPile<AzureBowCard>(
                PileType.Hand,
                position: CardPilePosition.Top
            );
    }
}
