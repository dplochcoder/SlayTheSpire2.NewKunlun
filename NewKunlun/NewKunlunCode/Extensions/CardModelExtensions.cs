using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace NewKunlun.NewKunlunCode.Extensions;

public static class CardModelExtensions
{
    extension(CardModel self)
    {
        public async Task AddGeneratedCardToPile<T>(
            PileType pileType,
            bool upgrade = false,
            CardPilePosition position = CardPilePosition.Bottom
        )
            where T : CardModel
        {
            CardCmd.PreviewCardPileAdd(
                await CardPileCmd.AddGeneratedCardToCombat(
                    self.CombatState!.CreateUpgradedCard<T>(self.Owner, upgrade),
                    pileType,
                    self.Owner,
                    position
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
