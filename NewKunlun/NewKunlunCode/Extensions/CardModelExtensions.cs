using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace NewKunlun.NewKunlunCode.Extensions;

public static class CardModelExtensions
{
    extension(CardModel self)
    {
        public async Task AddGeneratedStatusToPile<TStatus>(PileType pileType)
            where TStatus : CardModel
        {
            CardCmd.PreviewCardPileAdd(
                await CardPileCmd.AddGeneratedCardToCombat(
                    self.CombatState!.CreateCard<TStatus>(self.Owner),
                    PileType.Discard,
                    self.Owner
                )
            );
        }
    }

    extension<T>(T self)
        where T : CardModel
    {
        public T? Permanently(Action<T> action)
        {
            action(self);
            if (self.DeckVersion is T deckVersion)
            {
                action(deckVersion);
                return deckVersion;
            }

            return null;
        }
    }
}
