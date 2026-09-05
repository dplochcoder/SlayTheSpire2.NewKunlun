using System.Reflection;
using System.Runtime.CompilerServices;
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
            var stateMachineType = typeof(CardModel)
                .GetMethod(nameof(CardModel.OnPlayWrapper))
                ?.GetCustomAttribute<AsyncStateMachineAttribute>()
                ?.StateMachineType;
            var getResultLocationMethod = AccessTools.Method(
                typeof(CardModel),
                "GetResultLocationForCardPlay"
            );
            var endMethod = AccessTools.Method(
                typeof(CombatManager),
                nameof(CombatManager.EndCardOrPotionEffect)
            );
            var instanceGetter = AccessTools.PropertyGetter(
                typeof(CombatManager),
                nameof(CombatManager.Instance)
            );

            var foundEarlyCall = false;
            var readFieldName = false;
            var resultLocationFieldName = "";
            var foundEndMethod = false;
            var injectedCall = false;
            foreach (var instruction in src)
            {
                if (!foundEarlyCall)
                {
                    if (instruction.Calls(getResultLocationMethod))
                        foundEarlyCall = true;
                }
                else if (!readFieldName)
                {
                    resultLocationFieldName = (string)instruction.operand;
                    readFieldName = true;
                }
                else if (!foundEndMethod)
                {
                    if (instruction.Calls(endMethod))
                        foundEndMethod = true;
                }
                else if (!injectedCall)
                {
                    if (instruction.Calls(instanceGetter))
                    {
                        yield return CodeInstruction.LoadArgument(0); // this
                        yield return CodeInstruction.LoadLocal(1); // cardModel
                        yield return CodeInstruction.LoadField(
                            stateMachineType,
                            resultLocationFieldName
                        );
                        yield return CodeInstruction.Call(
                            (CardModel self, CardLocation resultLocation) =>
                                MaybeLateModifyResultLocation(self, resultLocation)
                        );
                        yield return CodeInstruction.StoreField(
                            stateMachineType,
                            resultLocationFieldName
                        );

                        injectedCall = true;
                    }
                }

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
