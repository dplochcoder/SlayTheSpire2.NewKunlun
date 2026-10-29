using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace NewKunlun.NewKunlunCode.Extensions;

public static class PlayerExtensions
{
    extension(Player self)
    {
        public CardModel Strike() =>
            self.Character.CardPool.AllCards.First(c =>
                c.Rarity == CardRarity.Basic && c.Tags.Contains(CardTag.Strike)
            );

        public CardModel Defend() =>
            self.Character.CardPool.AllCards.First(c =>
                c.Rarity == CardRarity.Basic && c.Tags.Contains(CardTag.Defend)
            );

        public Task AddCardToDeck<T>(bool upgrade = false)
            where T : CardModel => self.AddCardToDeck(ModelDb.Card<T>(), upgrade);

        public Task AddCardToDeck(CardModel canonical, bool upgrade = false) =>
            self.AddCardsToDeck([canonical], upgrade);

        public Task AddCardsToDeck<T>(int count, bool upgrade = false)
            where T : CardModel
        {
            List<CardModel> cards = [];
            for (var i = 0; i < count; i++)
                cards.Add(ModelDb.Card<T>());
            return self.AddCardsToDeck(cards, upgrade);
        }

        public async Task AddCardsToDeck(IEnumerable<CardModel> canonical, bool upgrade = false)
        {
            List<CardModel> cards = [.. canonical.Select(c => self.RunState.CreateCard(c, self))];
            if (upgrade)
            {
                foreach (var card in cards)
                    CardCmd.Upgrade(card);
            }

            CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(cards, PileType.Deck), 2f);
        }

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

        public bool PlayedAnyThisTurn(Func<CardPlay, bool> test) =>
            CombatManager.Instance.History.CardPlaysFinished.Any(e =>
                e.Actor == self.Creature
                && e.HappenedThisTurn(self.Creature.CombatState)
                && test(e.CardPlay)
            );

        public bool PlayedAnyCardThisTurn(Func<CardModel, bool> test) =>
            self.PlayedAnyThisTurn(p => test(p.Card));
    }
}
