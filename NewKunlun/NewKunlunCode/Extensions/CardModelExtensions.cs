using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace NewKunlun.NewKunlunCode.Extensions;

public static class CardModelExtensions
{
    extension<T>(T self)
        where T : CardModel
    {
        public uint NetId => NetCombatCard.FromModel(self).CombatCardIndex;

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
