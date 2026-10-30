using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Potions;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Necroceps",
    description: "On death, inflict [gold]Poison[/gold] equal to your max HP on your attacker.\nOnce per combat, after you die, revive at 10% of your max HP.",
    flavor: ""
)]
public partial class NecrocepsRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    [SavedProperty]
    public bool UsedThisCombat
    {
        get;
        set
        {
            AssertMutable();
            field = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override bool ShowCounter => true;

    public override int DisplayAmount => UsedThisCombat ? 1 : 0;

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource
    )
    {
        if (
            target != Owner.Creature
            || !result.WasTargetKilled
            || !props.IsCardOrMonsterMove()
            || dealer == null
        )
            return;

        Flash();
        await PowerCmd.Apply<PoisonPower>(choiceContext, dealer, target.MaxHp, target, null);
    }

    public override bool ShouldDie(Creature creature) =>
        creature != Owner.Creature || UsedThisCombat;

    public override async Task AfterPreventingDeath(Creature creature)
    {
        Flash();
        UsedThisCombat = true;
        await CreatureCmd.Heal(creature, Math.Max(creature.MaxHp / 10, 1));
    }

    [HarmonyPatch]
    private static class Patches
    {
        // Prefer to exhaust Necroceps before Fairy, since Necroceps replenishes.
        [HarmonyPatch(typeof(FairyInABottle), nameof(FairyInABottle.ShouldDie), MethodType.Getter)]
        [HarmonyPostfix]
        private static void ShouldDie(
            // ReSharper disable once InconsistentNaming
            FairyInABottle __instance,
            // ReSharper disable once InconsistentNaming
            ref bool __result
        )
        {
            if (__instance.Owner.Relics.Any(r => r is NecrocepsRelic { UsedThisCombat: false }))
                __result = false;
        }
    }
}
