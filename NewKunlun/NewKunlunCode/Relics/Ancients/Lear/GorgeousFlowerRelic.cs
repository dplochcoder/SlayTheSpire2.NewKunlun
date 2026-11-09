using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using NewKunlun.NewKunlunCode.Cards.Yi;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Lear;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Gorgeous Flower",
    description: "On pickup, remove an [gold]Eternal[/gold] curse from your deck.\nThe first time you shuffle your deck each combat, add [gold]Unto Soil[/gold] into your hand.",
    flavor: ""
)]
public class GorgeousFlowerRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    private static bool IsEternalCurse(CardModel card) =>
        card.Type == CardType.Curse && card.Keywords.Contains(CardKeyword.Eternal);

    public override bool IsAllowedForPlayer(Player player) =>
        player.Deck.Cards.Any(c =>
            c is not AscendersBane
            && c.Type == CardType.Curse
            && c.Keywords.Contains(CardKeyword.Eternal)
        );

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromKeyword(CardKeyword.Eternal),
            HoverTipFactory.FromCard<UntoSoilCard>(),
        ];

    public override bool HasUponPickupEffect => true;

    public override Task AfterObtained() =>
        CardSelectCmd.FromDeckForRemoval(
            Owner,
            new(CardSelectorPrefs.RemoveSelectionPrompt, 1),
            c => c.Type == CardType.Curse && c.Keywords.Contains(CardKeyword.Eternal)
        );

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
        if (shuffler != Owner || ++ShufflesThisCombat != 1)
            return;

        await Owner.AddGeneratedCardToPile<UntoSoilCard>(
            PileType.Hand,
            position: CardPilePosition.Top
        );
    }
}
