using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Heroic",
    description: "Gain {Energy:energyIcons()} per turn.\nDraw {Cards} additional cards per turn.\nGain {Strength} [gold]Strength[/gold] and {Dexterity} [gold]Dexterity[/gold].\nOnly 1 Player can be [gold]Heroic[/gold]."
)]
public partial class HeroicPower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new EnergyVar(2),
            new(nameof(Cards), 2),
            new PowerVar<StrengthPower>(nameof(Strength), 9),
            new PowerVar<DexterityPower>(nameof(Dexterity), 5),
        ];

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (Owner.Player is not { } player || Owner.CombatState == null)
            return;

        await PowerCmd.Apply<StrengthPower>(
            new ThrowingPlayerChoiceContext(),
            Owner,
            Strength.IntValue,
            Owner,
            cardSource
        );
        await PowerCmd.Apply<DexterityPower>(
            new ThrowingPlayerChoiceContext(),
            Owner,
            Dexterity.IntValue,
            Owner,
            cardSource
        );

        foreach (var ally in Owner.CombatState.Allies.Where(c => c != Owner))
            await PowerCmd.Remove<HeroicPower>(ally);
    }

    public override async Task AfterRemoved(Creature oldOwner)
    {
        await PowerCmd.Apply<StrengthPower>(
            new ThrowingPlayerChoiceContext(),
            oldOwner,
            -Strength.IntValue,
            oldOwner,
            null
        );
        await PowerCmd.Apply<DexterityPower>(
            new ThrowingPlayerChoiceContext(),
            oldOwner,
            -Dexterity.IntValue,
            oldOwner,
            null
        );
    }

    public override decimal ModifyMaxEnergy(Player player, decimal amount) =>
        player.Creature == Owner ? amount + Energy.IntValue : amount;

    public override decimal ModifyHandDraw(Player player, decimal count) =>
        player.Creature == Owner ? count + Cards.IntValue : count;
}
