using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Abacus;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Bunsen Burner",
    description: "Whenever you [gold]Smith[/gold], the cards you upgrade gain random enchantments.",
    flavor: ""
)]
public class BunsenBurnerRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    [HarmonyPatch]
    private static class Patches
    {
        [HarmonyPatch(typeof(SmithRestSiteOption), nameof(SmithRestSiteOption.OnSelect))]
        [HarmonyPostfix]
        private static void OnSelect(
            // ReSharper disable once InconsistentNaming
            SmithRestSiteOption __instance,
            // ReSharper disable once InconsistentNaming
            ref Task<bool> __result
        ) => __result = WrapOnSelect(__instance, __result);

        private static async Task<bool> WrapOnSelect(SmithRestSiteOption option, Task<bool> result)
        {
            if (!await result)
                return false;

            var relic = option.Owner.Relics.OfType<BunsenBurnerRelic>().FirstOrDefault();
            if (relic == null)
                return true;

            if (
                (option._selection ?? []).Aggregate(
                    false,
                    (current, card) =>
                        current
                        | RandomEnchantCmd.ApplyEnchantment(
                            option.Owner,
                            option.Owner.RunState.Rng.Niche,
                            card
                        )
                )
            )
                relic.Flash();
            return true;
        }
    }
}
