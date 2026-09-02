using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace NewKunlun.NewKunlunCode.Extensions;

public static class PlayerExtensions
{
    extension(Player self)
    {
        public T? FindCard<T>(IEnumerable<PileType> searchOrder)
            where T : CardModel
        {
            foreach (var pileType in searchOrder)
            {
                List<T> cards = [.. pileType.GetPile(self).Cards.OfType<T>()];

                if (cards.FirstOrDefault(c => c.IsUpgraded) is { } upgradedCard)
                    return upgradedCard;
                if (cards.FirstOrDefault() is { } card)
                    return card;
            }

            return null;
        }
    }
}
