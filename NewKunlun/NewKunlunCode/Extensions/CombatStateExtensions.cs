using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace NewKunlun.NewKunlunCode.Extensions;

public static class CombatStateExtensions
{
    extension(ICombatState self)
    {
        public T CreateUpgradedCard<T>(Player owner, bool upgrade)
            where T : CardModel
        {
            var card = self.CreateCard<T>(owner);
            if (upgrade)
                CardCmd.Upgrade(card);
            return card;
        }
    }
}
