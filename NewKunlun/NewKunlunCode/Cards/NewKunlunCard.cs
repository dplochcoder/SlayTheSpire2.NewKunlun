using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Keywords;

namespace NewKunlun.NewKunlunCode.Cards;

/// <summary>
/// This is the base class for your mod's cards, which is set up to load the card's images from your mod's resources.
/// When creating a card, right click the Cards folder and create a new file with the Custom Card template.
/// This will generate a class that extends this one.
/// You can also just create the class manually; just make sure to inherit from this class.
/// </summary>
public abstract class NewKunlunCard(int cost, CardType type, CardRarity rarity, TargetType target)
    : CustomCardModel(cost, type, rarity, target)
{
    public virtual bool IsParryCard => false;

    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    public override string CustomPortraitPath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    //Smaller variants of card images for efficiency:
    //Smaller variant of fullart: 250x350
    //Smaller variant of normalart: 250x190

    //Uses card_portraits/card_name.png as image path. These should be smaller images.
    public override string PortraitPath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath =>
        $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    protected override CardLocation GetResultLocationForCardPlay()
    {
        var result = base.GetResultLocationForCardPlay();
        if (Keywords.Contains(CustomCardKeyword.Ephemeral))
            result.pileType = PileType.Exhaust;

        return result;
    }

    //Ephemeral handling.
    public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
    {
        await base.AfterCardDiscarded(choiceContext, card);

        if (card == this && card.Keywords.Contains(CustomCardKeyword.Ephemeral))
            await CardCmd.Exhaust(choiceContext, card, causedByEthereal: true);
    }

    [SavedProperty]
    public bool ExhaustAtEndOfTurn
    {
        get;
        set
        {
            AssertMutable();
            field = value;
        }
    } = false;

    public override async Task AfterCardDrawn(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool fromHandDraw
    )
    {
        if (card == this && card.Keywords.Contains(CustomCardKeyword.Ephemeral))
            ExhaustAtEndOfTurn = true;

        await base.AfterCardDrawn(choiceContext, card, fromHandDraw);
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        IReadOnlyList<Creature> participantsList = [.. participants];
        if (ExhaustAtEndOfTurn && participantsList.Contains(Owner.Creature))
            await CardCmd.Exhaust(choiceContext, this);

        await base.AfterSideTurnEnd(choiceContext, side, participantsList);
    }
}
