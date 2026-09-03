using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;

namespace NewKunlun.NewKunlunCode.Combat.History;

public sealed class TalismanRemovedEntry(
    Creature owner,
    Creature? applier,
    int roundNumber,
    CombatSide currentSide,
    CombatHistory history,
    IEnumerable<Player> players
) : CombatHistoryEntry(owner, roundNumber, currentSide, history, players)
{
    public Creature? Applier { get; } = applier;

    public override string Description =>
        $"{Applier?.ModelId.Entry ?? "Unknown"}'s Talisman was removed from {Actor.ModelId.Entry}";
}
