using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Saves.Runs;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Medical Jade",
    description: "When you rest, instead of healing, gain {Regen} [gold]Regen[/gold], {PillowRegen} if you have [gold]Regal Pillow[/gold].\nYour [gold]Regen[/gold] persists between combats.",
    flavor: ""
)]
public partial class MedicalJadeRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new PowerVar<RegenPower>(nameof(Regen), 8),
            new PowerVar<RegenPower>(nameof(PillowRegen), 10),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<RegenPower>()];

    private int RegenAmount =>
        Owner.Relics.Any(r => r is RegalPillow) ? PillowRegen.IntValue : Regen.IntValue;

    [SavedProperty]
    public int CurrentRegen
    {
        get;
        set
        {
            AssertMutable();
            field = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override bool ShowCounter => CurrentRegen > 0;

    public override int DisplayAmount => CurrentRegen;

    public override Task AfterRestSiteHeal(Player player, bool isMimicked)
    {
        if (player != Owner)
            return Task.CompletedTask;

        Flash();
        CurrentRegen += RegenAmount;
        return Task.CompletedTask;
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (side != Owner.Creature.Side || CurrentRegen == 0 || combatState.RoundNumber != 1)
            return;

        Flash();
        await PowerCmd.Apply<RegenPower>(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            CurrentRegen,
            Owner.Creature,
            null
        );
    }

    public override Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource
    )
    {
        if (power is not RegenPower regen || power.Owner != Owner.Creature)
            return Task.CompletedTask;

        CurrentRegen = regen.Amount;
        return Task.CompletedTask;
    }

    [HarmonyPatch]
    private static class Patches
    {
        [HarmonyPatch(typeof(HealRestSiteOption), nameof(HealRestSiteOption.GetHealAmount))]
        [HarmonyPostfix]
        private static void GetHealAmount(
            // ReSharper disable once InconsistentNaming
            HealRestSiteOption __instance,
            Player player,
            // ReSharper disable once InconsistentNaming
            ref decimal __result
        )
        {
            if (player.Relics.Any(r => r is MedicalJadeRelic))
                __result = 0;
        }
    }
}
