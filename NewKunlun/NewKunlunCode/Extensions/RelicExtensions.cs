using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs.History;

namespace NewKunlun.NewKunlunCode.Extensions;

public static class RelicExtensions
{
    extension<T>(T self)
        where T : RelicModel
    {
        public async Task ChooseCardToAddToDeck(List<CardModel> cards)
        {
            var chosen = await CardSelectCmd.FromChooseACardScreen(
                new BlockingPlayerChoiceContext(),
                cards,
                self.Owner
            );
            CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(chosen!, PileType.Deck));
            foreach (var card in cards.Where(card => card != chosen))
                self.Owner.RunState.CurrentMapPointHistoryEntry?.GetEntry(self.Owner.NetId)
                    .CardChoices.Add(new CardChoiceHistoryEntry(card, false));
        }

        public bool IsNextUnused => self == self.NextUnused;

        public T? NextUnused => self.Owner.Relics.OfType<T>().FirstOrDefault(r => !r.IsUsedUp);
    }
}
