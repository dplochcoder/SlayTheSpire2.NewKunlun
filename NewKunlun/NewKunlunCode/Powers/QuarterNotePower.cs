using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Quarter-Note",
    description: "When you have {Total} or more, gain {Total:energyIcons()}, draw {Total} cards, and lose {Total} [gold]Quarter-Note[/gold]."
)]
public partial class QuarterNotePower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new(nameof(Total), 4)];

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource
    )
    {
        while (Owner.Player != null && power.Amount >= Total.IntValue)
        {
            await PlayerCmd.GainEnergy(Total.IntValue, Owner.Player);
            await CardPileCmd.Draw(choiceContext, Total.IntValue, Owner.Player);
            await PowerCmd.ModifyAmount(choiceContext, this, -Total.IntValue, Owner, null);
        }
    }
}
