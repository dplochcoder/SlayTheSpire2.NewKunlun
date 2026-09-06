using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(title: "Talisman Detonated this Turn", description: "INTERNAL")]
public class TalismanDetonatedThisTurnPower : NewKunlunPower
{
    public override PowerType Type => PowerType.None;
    public override PowerStackType StackType => PowerStackType.Single;
    protected override bool IsVisibleInternal => false;

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (!participants.Contains(Owner))
            return;

        await PowerCmd.Remove(this);
    }
}
