using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves.Runs;
using NewKunlun.NewKunlunCode.Keywords;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Modifiers;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Shuanshuan;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Virtual Reality Device",
    description: "Gain {Energy:energyIcons()} each turn.\nThe first card you draw each turn is [gold]Quantum[/gold].",
    flavor: ""
)]
public partial class VirtualRealityDeviceRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(ExtraCardKeywords.Quantum)];

    public override decimal ModifyMaxEnergy(Player player, decimal amount) =>
        player == Owner ? amount + 1 : amount;

    [SavedProperty]
    public int QuantumThisTurn
    {
        get;
        set
        {
            AssertMutable();
            field = value;
        }
    }

    public override Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState
    )
    {
        QuantumThisTurn = 0;
        return Task.CompletedTask;
    }

    public override Task AfterCardDrawn(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool fromHandDraw
    )
    {
        if (card.Owner != Owner || QuantumThisTurn > 0)
            return Task.CompletedTask;

        ++QuantumThisTurn;
        CardModifier.AddModifier<QuantumModifier>(card);
        CardCmd.Preview(card);
        return Task.CompletedTask;
    }
}
