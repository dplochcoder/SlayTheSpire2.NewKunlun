using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using NewKunlun.NewKunlunCode.Cards.Yi;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Lear;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Potted Plant",
    description: "On pickup, remove all [gold]Curses[/gold] from your deck, even if they're [gold]Eternal[/gold].\nThe first time you shuffle your deck each combat, add [gold]Unto Soil[/gold] into your hand.",
    flavor: ""
)]
public class PottedPlantRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    private static bool IsEternalCurse(CardModel card) =>
        card.Type == CardType.Curse && card.Keywords.Contains(CardKeyword.Eternal);

    public override bool IsAllowedForPlayer(Player player)
    {
        List<CardModel> curses = [.. player.Deck.Cards.Where(c => c.Type == CardType.Curse)];
        return curses.Count >= 2 || curses.Any(c => c.Keywords.Contains(CardKeyword.Eternal));
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromKeyword(CardKeyword.Eternal),
            HoverTipFactory.FromCard<UntoSoilCard>(),
        ];

    public override bool HasUponPickupEffect => true;

    public override Task AfterObtained() =>
        CardPileCmd.RemoveFromDeck([.. Owner.Deck.Cards.Where(c => c.Type == CardType.Curse)]);

    [SavedProperty]
    public int ShufflesThisCombat
    {
        get;
        set
        {
            AssertMutable();
            field = value;
        }
    }

    public override Task BeforeRoomEntered(AbstractRoom room)
    {
        ShufflesThisCombat = 0;
        return Task.CompletedTask;
    }

    public override async Task AfterShuffle(PlayerChoiceContext choiceContext, Player shuffler)
    {
        if (shuffler != Owner || ++ShufflesThisCombat != 2)
            return;

        await Owner.AddGeneratedCardToPile<UntoSoilCard>(
            PileType.Hand,
            position: CardPilePosition.Top
        );
    }
}
