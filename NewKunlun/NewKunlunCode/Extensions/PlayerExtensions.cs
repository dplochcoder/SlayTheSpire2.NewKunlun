using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace NewKunlun.NewKunlunCode.Extensions;

public static class PlayerExtensions
{
    extension(Player self)
    {
        public async Task AddGeneratedCardToPile<T>(
            PileType pileType,
            bool upgrade = false,
            CardPilePosition position = CardPilePosition.Bottom
        )
            where T : CardModel
        {
            if (self.Creature.CombatState == null)
                return;

            CardCmd.PreviewCardPileAdd(
                await CardPileCmd.AddGeneratedCardToCombat(
                    self.Creature.CombatState.CreateUpgradedCard<T>(self, upgrade),
                    pileType,
                    self,
                    position
                )
            );
        }

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
