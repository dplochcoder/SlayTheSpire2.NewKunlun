using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Lear;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Adamantium",
    description: "If you end your turn with at least {Energy:energyIcons()}, gain {Block} [gold]Block[/gold].",
    flavor: ""
)]
public partial class AdamantiumRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new EnergyVar(1), new BlockVar(15, ValueProp.Unpowered)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.Static(StaticHoverTip.Block)];

    public override Task BeforeCombatStart()
    {
        Owner.PlayerCombatState?.EnergyChanged += OnEnergyChanged;
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        Owner.PlayerCombatState?.EnergyChanged -= OnEnergyChanged;
        return Task.CompletedTask;
    }

    private void OnEnergyChanged(int prev, int next) =>
        Status = next > 0 ? RelicStatus.Active : RelicStatus.Normal;

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (side != Owner.Creature.Side || Owner.PlayerCombatState is not { Energy: > 0 })
            return;

        Flash();
        Status = RelicStatus.Normal;
        await CreatureCmd.GainBlock(Owner.Creature, Block, null);
    }
}
