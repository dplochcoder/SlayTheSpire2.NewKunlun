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
            var earlyCall = AccessTools.Method(typeof(CardModel), "GetResultLocationForCardPlay");

            var instructions = src.ToList();
            var earlyCallIndex = instructions.FindIndex(i => i.Calls(earlyCall));
            if (earlyCallIndex <= 0 || earlyCallIndex >= instructions.Count - 1)
                throw new InvalidOperationException(
                    "Could not find GetResultLocationForCardPlay() call."
                );

            var cardLocalIndex = instructions[earlyCallIndex - 1].LocalIndex();
            var resultLocationLocalIndex = instructions[earlyCallIndex + 1].LocalIndex();
            if (cardLocalIndex < 0 || resultLocationLocalIndex < 0)
                throw new InvalidOperationException("Could not find required locals.");

            var endMethod = AccessTools.Method(
                typeof(CombatManager),
                nameof(CombatManager.EndCardOrPotionEffect)
            );
            var instanceGetter = AccessTools.PropertyGetter(
                typeof(CombatManager),
                nameof(CombatManager.Instance)
            );
            var foundEndMethod = false;
            var injectedCall = false;
            foreach (var instruction in instructions)
            {
                if (injectedCall)
                    yield return instruction;
                else if (!foundEndMethod && instruction.Calls(endMethod))
                {
                    foundEndMethod = true;
                    continue;
                }
                else if (foundEndMethod && instruction.Calls(instanceGetter))
                {
                    var loadCard = CodeInstruction.LoadLocal(cardLocalIndex);
                    instruction.MoveLabelsTo(loadCard);

                    yield return loadCard;
                    yield return CodeInstruction.LoadLocal(resultLocationLocalIndex);
                    yield return CodeInstruction.Call(
                        (CardModel self, CardLocation resultLocation) =>
                            MaybeLateModifyResultLocation(self, resultLocation)
                    );
                    yield return CodeInstruction.StoreLocal(resultLocationLocalIndex);

                    injectedCall = true;
                }
                else
                    yield return instruction;
            }

            if (!injectedCall)
                throw new InvalidOperationException(
                    "Could not inject ILateModifyResultLocation hook."
                );
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
