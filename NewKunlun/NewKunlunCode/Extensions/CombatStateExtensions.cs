using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace NewKunlun.NewKunlunCode.Extensions;

public static class CombatStateExtensions
{
    extension(ICombatState self)
    {
        public T CreateUpgradedCard<T>(T card, Player owner, bool upgrade)
            where T : CardModel
        {
            card = (T)self.CreateCard(card, owner);
            if (upgrade)
                CardCmd.Upgrade(card);
            return card;
        }

        public T CreateUpgradedCard<T>(Player owner, bool upgrade)
            where T : CardModel => self.CreateUpgradedCard(ModelDb.Card<T>(), owner, upgrade);
    }
}
