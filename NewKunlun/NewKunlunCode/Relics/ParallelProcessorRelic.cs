using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Parallel Processor",
    description: "You can no longer [gold]Smith[/gold] at rest sites.\nWhenever you [gold]Rest[/gold], upgrade a random card.",
    flavor: "",
    additionalRestSiteHealText: "Upgrade a random card."
)]
public class ParallelProcessorRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool TryModifyRestSiteOptions(
        Player player,
        ICollection<RestSiteOption> options
    )
    {
        if (options.OfType<SmithRestSiteOption>().FirstOrDefault() is { } smith)
        {
            options.Remove(smith);
            return true;
        }

        return false;
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is RestSiteRoom && PileType.Deck.GetPile(Owner).Cards.Any(c => c.IsUpgradable))
            Status = RelicStatus.Active;
        else
            Status = RelicStatus.Normal;

        return Task.CompletedTask;
    }

    public override Task AfterRestSiteHeal(Player player, bool isMimicked)
    {
        if (player != Owner)
            return Task.CompletedTask;

        List<CardModel> cards = [.. PileType.Deck.GetPile(Owner).Cards.Where(c => c.IsUpgradable)];
        if (cards.Count == 0)
            return Task.CompletedTask;

        Flash();
        CardCmd.Upgrade(player.PlayerRng.Rewards.NextItem(cards)!);
        return Task.CompletedTask;
    }

    public override IReadOnlyList<LocString> ModifyExtraRestSiteHealText(
        Player player,
        IReadOnlyList<LocString> currentExtraText
    ) =>
        LocalContext.IsMe(Owner)
            ? [.. currentExtraText.Concat([AdditionalRestSiteHealText!])]
            : currentExtraText;
}
