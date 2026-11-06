using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace NewKunlun.NewKunlunCode.Extensions;

public static class CardPoolModelExtensions
{
    extension(CardPoolModel self)
    {
        public IEnumerable<CardModel> CardsUnlockedFor(Player player) =>
            self.GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint);
    }
}
