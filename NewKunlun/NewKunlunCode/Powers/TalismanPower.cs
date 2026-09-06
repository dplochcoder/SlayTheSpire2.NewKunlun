using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Cards;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Talisman",
    description: "[gold]Marked[/gold] by your Talisman.\nRemoved after 2 turns, or when you [gold]Detonate[/gold].",
    smartDescription: "[gold]Marked[/gold] by your Talisman.\nRemoved after {TurnsRemaining} {TurnsRemaining:plural:turn|turns}, or when you [gold]Detonate[/gold].",
    remoteDescription: "[gold]Marked[/gold] by another player's Talisman."
)]
public partial class TalismanPower : NewKunlunPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar(nameof(TurnsRemaining), 2M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.Mark(), Tip.Detonate()];

    private static IReadOnlyList<TalismanPower> GetAll(Player player)
    {
        if (player.Creature.CombatState == null)
            return [];

        return
        [
            .. player.Creature.CombatState.Enemies.SelectMany(e =>
                e.GetPowerInstances<TalismanPower>().Where(p => p.Applier == player.Creature)
            ),
        ];
    }

    public static async Task RemoveAll(Player player)
    {
        foreach (var power in GetAll(player))
            await PowerCmd.Remove(power);
    }

    public static async Task DetonateAll(
        PlayerChoiceContext choiceContext,
        TalismanDetonateCard cardSource
    )
    {
        foreach (var power in GetAll(cardSource.Owner))
        {
            await PowerCmd.Apply<TalismanDetonatedThisTurnPower>(
                choiceContext,
                power.Owner,
                1M,
                cardSource.Owner.Creature,
                cardSource
            );
            await PowerCmd.Remove(power);
        }
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (!participants.Contains(Owner))
            return;
        if (--TurnsRemaining.BaseValue > 0)
            return;

        await PowerCmd.Remove(this);
        Flash();
    }
}
