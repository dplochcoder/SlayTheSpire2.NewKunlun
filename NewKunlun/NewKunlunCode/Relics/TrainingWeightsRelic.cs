using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves.Runs;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Training Weights",
    description: "Remove all your [gold]Strikes[/gold] and [gold]Defends[/gold].\nAt the start of combat, gain 1 [gold]Strength[/gold] for each [gold]Strike[/gold] and 1 [gold]Dexterity[/gold] for each [gold]Defend[/gold] removed.",
    flavor: ""
)]
public class TrainingWeightsRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool IsAllowedForPlayer(Player player) =>
        player.Deck.Cards.Count(c => c.IsBasicStrikeOrDefend) >= 4;

    public override bool HasUponPickupEffect => true;

    [SavedProperty]
    public int StrikesRemoved
    {
        get;
        set
        {
            AssertMutable();
            field = value;
        }
    }

    [SavedProperty]
    public int DefendsRemoved
    {
        get;
        set
        {
            AssertMutable();
            field = value;
        }
    }

    public override async Task AfterObtained()
    {
        IReadOnlyList<CardModel> cards =
        [
            .. Owner.Deck.Cards.Where(c => c is { IsBasicStrikeOrDefend: true, IsRemovable: true }),
        ];
        await CardPileCmd.RemoveFromDeck(cards);
        StrikesRemoved = cards.Count(c => c.Tags.Contains(CardTag.Strike));
        DefendsRemoved = cards.Count(c => c.Tags.Contains(CardTag.Defend));
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (
            side != Owner.Creature.Side
            || combatState.RoundNumber != 1
            || StrikesRemoved + DefendsRemoved <= 0
        )
            return;

        Flash();
        await PowerCmd.Apply<StrengthPower>(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            StrikesRemoved,
            Owner.Creature,
            null
        );
        await PowerCmd.Apply<DexterityPower>(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            DefendsRemoved,
            Owner.Creature,
            null
        );
    }
}
