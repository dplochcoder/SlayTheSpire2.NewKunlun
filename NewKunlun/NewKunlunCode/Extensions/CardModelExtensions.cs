using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace NewKunlun.NewKunlunCode.Extensions;

public static class CardModelExtensions
{
    public static async Task AddGeneratedStatusToPile<T>(
        this CardModel self,
        PileType pileType = PileType.Discard
    )
        where T : CardModel
    {
        CardCmd.PreviewCardPileAdd(
            await CardPileCmd.AddGeneratedCardToCombat(
                self.CombatState!.CreateCard<T>(self.Owner),
                PileType.Discard,
                self.Owner
            )
        );
        await Cmd.Wait(0.5f);
    }
}
