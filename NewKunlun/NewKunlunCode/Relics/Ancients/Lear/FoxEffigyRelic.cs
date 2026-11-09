using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Lear;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Fox Effigy",
    description: "Every seventh card you play is free.",
    flavor: ""
)]
public class FoxEffigyRelic : NewKunlunRelic
{
    private const int NumPlays = 7;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    [SavedProperty]
    public int NextCardPlay
    {
        get;
        set
        {
            AssertMutable();
            field = value;
            Status = NextCardPlay == NumPlays ? RelicStatus.Active : RelicStatus.Normal;
        }
    }

    public override bool ShowCounter => true;

    public override int DisplayAmount => NextCardPlay;

    public override Task BeforeCombatStart()
    {
        Status = NextCardPlay == NumPlays ? RelicStatus.Active : RelicStatus.Normal;
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        Status = RelicStatus.Active;
        return Task.CompletedTask;
    }

    public override bool TryModifyEnergyCostInCombatLate(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost
    )
    {
        modifiedCost = originalCost;
        if (
            card.Owner != Owner
            || NextCardPlay != NumPlays
            || card.Pile?.Type is not (PileType.Hand or PileType.Play)
        )
            return false;

        modifiedCost = 0;
        return true;
    }

    public override bool TryModifyStarCost(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost
    )
    {
        modifiedCost = originalCost;
        if (
            card.Owner != Owner
            || NextCardPlay != NumPlays
            || card.Pile?.Type is not (PileType.Hand or PileType.Play)
        )
            return false;

        modifiedCost = 0;
        return true;
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner)
            return Task.CompletedTask;

        if (NextCardPlay == NumPlays)
        {
            Flash();
            NextCardPlay = 1;
        }
        else
            ++NextCardPlay;

        return Task.CompletedTask;
    }
}
