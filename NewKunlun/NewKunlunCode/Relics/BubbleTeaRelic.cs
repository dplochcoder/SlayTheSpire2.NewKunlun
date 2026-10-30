using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Bubble Tea",
    description: "At the start of combat, gain {Energy:energyIcons()} and draw {Cards} additional cards.",
    flavor: ""
)]
public partial class BubbleTeaRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new EnergyVar(3), new(nameof(Cards), 2)];

    public override decimal ModifyHandDraw(Player player, decimal count) =>
        player != Owner || Owner.PlayerCombatState?.TurnNumber > 1
            ? count
            : count + Energy.IntValue;

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (side != Owner.Creature.Side || Owner.PlayerCombatState?.TurnNumber > 1)
            return;

        Flash();
        await PlayerCmd.GainEnergy(Energy.IntValue, Owner);
    }
}
