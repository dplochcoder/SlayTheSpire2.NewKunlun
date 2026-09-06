using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Speed",
    description: "At the start of your next 3 turns, draw {Amount:plural:1 additional card|{Amount} additional cards}."
)]
public partial class SpeedPower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar(nameof(TurnsRemaining), 3M)];

    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if (player.Creature != Owner)
            return count;

        return count + Amount;
    }

    public override async Task AfterModifyingHandDraw()
    {
        Flash();
        if (--TurnsRemaining.BaseValue <= 0)
            await PowerCmd.Remove(this);
    }
}
