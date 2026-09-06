using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Vitality",
    description: "At the start of your next 3 turns, gain {Amount:energyIcons()}.",
    smartDescription: "At the start of your next {TurnsRemaining:plural:turn|{TurnsRemaining} turns}, gain {Amount:energyIcons()}."
)]
public partial class VitalityPower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar(nameof(TurnsRemaining), 3M)];

    public override async Task AfterEnergyReset(Player player)
    {
        Flash();
        await PlayerCmd.GainEnergy(Amount, player);
        if (--TurnsRemaining.BaseValue <= 0)
            await PowerCmd.Remove(this);
    }
}
