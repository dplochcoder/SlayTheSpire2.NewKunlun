using System.Collections;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace NewKunlun.NewKunlunCode.Extensions;

public static class CardModelExtensions
{
    extension(CardModel self)
    {
        public CardKeywordList BuildKeywords() => new(self, []);

        public CardKeywordList BuildKeywords(CardKeyword keyword) => new(self, [keyword]);

        public CardKeywordList BuildKeywords(IEnumerable<CardKeyword> keywords) =>
            new(self, keywords);

        public async Task AddGeneratedStatusToPile<T>(PileType pileType = PileType.Discard)
            where T : CardModel
        {
            CardCmd.PreviewCardPileAdd(
                await CardPileCmd.AddGeneratedCardToCombat(
                    self.CombatState!.CreateCard<T>(self.Owner),
                    PileType.Discard,
                    self.Owner
                )
            );
        }
    }

    public class CardKeywordList(CardModel model, IEnumerable<CardKeyword> list)
        : IEnumerable<CardKeyword>
    {
        public CardKeywordList If(Func<bool> condition, IEnumerable<CardKeyword> add)
        {
            IEnumerable<CardKeyword> Gen()
            {
                foreach (var keyword in list)
                    yield return keyword;
                if (condition())
                {
                    foreach (var keyword in add)
                        yield return keyword;
                }
            }

            return new CardKeywordList(model, Gen());
        }

        public CardKeywordList If(Func<bool> condition, CardKeyword add) => If(condition, [add]);

        public CardKeywordList IfUpgraded(IEnumerable<CardKeyword> add) =>
            If(() => model.IsUpgraded, add);

        public CardKeywordList IfUpgraded(CardKeyword add) => IfUpgraded([add]);

        public IEnumerator<CardKeyword> GetEnumerator() => list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();
    }
}
