using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Commands;

namespace NewKunlun.NewKunlunCode.Relics;

public abstract class EnchantCardsRelic<T> : NewKunlunRelic
    where T : EnchantmentModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        HoverTipFactory.FromEnchantment<T>(EnchantAmount);

    public sealed override bool HasUponPickupEffect => true;

    protected virtual int EnchantAmount => 1;

    protected virtual int NumCardsMin => 0;

    protected abstract int NumCardsMax { get; }

    public override bool IsAllowedForPlayer(Player player) =>
        player.Deck.Cards.Count(ModelDb.Enchantment<T>().CanEnchant) >= NumCardsMax;

    public sealed override Task AfterObtained() =>
        ExtraCardSelectCmds.EnchantFromDeck<T>(Owner, EnchantAmount, NumCardsMin, NumCardsMax);
}
