using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Lear;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Haotian Telescope",
    description: "At the start of combat, shuffle {Cards} random Colorless cards into your deck.\nThey are free to play this combat.",
    flavor: ""
)]
public partial class HaotianTelescopeRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new(nameof(Cards), 5)];

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (side != Owner.Creature.Side || combatState.RoundNumber != 1)
            return;

        Flash();
        List<CardModel> cards = [.. ModelDb.CardPool<ColorlessCardPool>().CardsUnlockedFor(Owner)];
        cards = [.. Owner.RunState.Rng.CombatCardGeneration.NextItems(Cards.IntValue, cards)];
        cards = [.. cards.Select(c => combatState.CreateCard(c, Owner))];
        foreach (var card in cards)
            card.SetToFreeThisCombat();
        CardCmd.PreviewCardPileAdd(
            await CardPileCmd.Add(
                cards,
                PileType.Draw.GetPile(Owner),
                position: CardPilePosition.Random,
                clonedBy: this
            )
        );
    }
}
