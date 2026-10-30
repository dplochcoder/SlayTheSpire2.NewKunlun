using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Solar Panel",
    description: "On every even turn of combat, gain {Energy:energyIcons()}.",
    flavor: ""
)]
public partial class SolarPanelRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(2)];

    public override bool ShowCounter => Owner.Creature.CombatState != null;

    public override int DisplayAmount => Owner.Creature.CombatState?.RoundNumber ?? 0;

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        InvokeDisplayAmountChanged();
        if (side != Owner.Creature.Side || Owner.Creature.CombatState?.RoundNumber % 2 == 1)
            return;

        Flash();
        await PlayerCmd.GainEnergy(Energy.IntValue, Owner);
    }
}
