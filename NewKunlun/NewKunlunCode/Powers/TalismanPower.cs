using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Cards;
using NewKunlun.NewKunlunCode.Combat.History;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Talisman",
    description: "[gold]Talisman Detonate[/gold] can be activated on this enemy. Removed after 2 turns or on detonate.",
    smartDescription: "{TalismanDetonate:cardName()} can be activated on this enemy. Removed after {TurnsRemaining} {TurnsRemaining:plural:turns|turn} or on detonate.",
    remoteDescription: "Another player can activate {TalismanDetonate:cardName()} on this enemy."
)]
public partial class TalismanPower : NewKunlunPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DynamicVar(nameof(TurnsRemaining), 2M),
            new TalismanDetonateVar<TalismanPower>(power =>
                TalismanDetonateCard.IsUpgradedAnywhere(power.Applier?.Player)
            ),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [Tip.TalismanDetonateCard(Applier?.Player)];

    public override Task AfterRemoved(Creature oldOwner)
    {
        var combatState = oldOwner.CombatState;
        if (combatState is null)
            return Task.CompletedTask;

        var history = CombatManager.Instance.History;
        history.Add(
            combatState,
            new TalismanRemovedEntry(
                oldOwner,
                Applier,
                combatState.RoundNumber,
                combatState.CurrentSide,
                history,
                combatState.Players
            )
        );

        return Task.CompletedTask;
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
