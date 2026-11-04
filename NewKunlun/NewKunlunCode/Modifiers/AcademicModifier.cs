using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Modifiers;

[CardModifierLocalization(
    title: "Academic",
    description: "Gains [gold]Replay[/gold] {Amount} this turn."
)]
public class AcademicModifier : NewKunlunModifier
{
    public override ModifierStackType StackType => ModifierStackType.Counter;

    public override Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (Owner == null || side != Owner.Owner.Creature.Side)
            return Task.CompletedTask;

        Owner.BaseReplayCount -= Amount;
        RemoveModifier(Owner, this);
        return Task.CompletedTask;
    }

    protected override void OnAmountChanged(int change) => Owner?.BaseReplayCount += change;
}
