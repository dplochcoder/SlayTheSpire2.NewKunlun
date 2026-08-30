using System.Reflection.Emit;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace NewKunlun.NewKunlunCode.Hooks;

public interface ILateModifyResultLocation
{
    void LateModifyResultLocation(ref CardLocation resultLocation);

    [HarmonyPatch]
    private static class Patches
    {
        [HarmonyPatch(typeof(CardModel), nameof(CardModel.OnPlayWrapper), MethodType.Async)]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpile(IEnumerable<CodeInstruction> src)
        {
            var earlyCall = typeof(CardModel).GetMethod("GetResultLocationForCardPlay");

            // Seek a unique method preceding the common method.
            var endMethod = typeof(CombatManager).GetMethod(
                nameof(CombatManager.EndCardOrPotionEffect)
            );
            var foundEndMethod = false;
            var insertedHook = false;

            var instanceGetter = AccessTools.PropertyGetter(
                typeof(CombatManager),
                nameof(CombatManager.Instance)
            );

            var nextInstrHasResultLocationAddr = false;
            int? resultLocationAddr = null;
            foreach (var instr in src)
            {
                if (resultLocationAddr != null)
                {
                    if (nextInstrHasResultLocationAddr)
                        resultLocationAddr = Convert.ToInt32(instr.operand);
                    else if (instr.Is(OpCodes.Callvirt, earlyCall))
                        nextInstrHasResultLocationAddr = true;
                }

                if (!foundEndMethod && instr.Is(OpCodes.Callvirt, endMethod))
                    foundEndMethod = true;

                if (
                    resultLocationAddr != null
                    && !insertedHook
                    && foundEndMethod
                    && instr.Calls(instanceGetter)
                )
                {
                    insertedHook = true;

                    yield return CodeInstruction.LoadLocal(1);
                    yield return CodeInstruction.LoadLocal(resultLocationAddr.Value);
                    yield return CodeInstruction.Call(
                        (CardModel self, CardLocation resultLocation) =>
                            MaybeLateModifyResultLocation(self, resultLocation)
                    );
                    yield return CodeInstruction.StoreLocal(resultLocationAddr.Value);
                }

                yield return instr;
            }
        }

        private static CardLocation MaybeLateModifyResultLocation(
            CardModel self,
            CardLocation resultLocation
        )
        {
            if (self is ILateModifyResultLocation lateModifier)
                lateModifier.LateModifyResultLocation(ref resultLocation);
            return resultLocation;
        }
    }
}
