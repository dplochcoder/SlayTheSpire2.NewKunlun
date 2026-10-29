using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace NewKunlun.NewKunlunCode.Afflictions;

public abstract class NewKunlunAffliction : AfflictionModel
{
    protected virtual string? CustomOverlayPath => null;

    [HarmonyPatch]
    private static class Patches
    {
        [HarmonyPatch(
            typeof(AfflictionModel),
            nameof(AfflictionModel.OverlayPath),
            MethodType.Getter
        )]
        [HarmonyPrefix]
        private static bool OverlayPath(
            // ReSharper disable once InconsistentNaming
            AfflictionModel __instance,
            // ReSharper disable once InconsistentNaming
            ref string? __result
        )
        {
            if (__instance is not NewKunlunAffliction affliction)
                return true;

            __result = affliction.CustomOverlayPath;
            return __result == null;
        }
    }
}
