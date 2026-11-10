using BaseLib.Patches.Content;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace NewKunlun.NewKunlunCode.Extensions;

public static class ExtraValueProps
{
    [CustomEnum]
    public static ValueProp Crimson;

    public static bool IsCrimsonAttack(this ValueProp self) => (self & Crimson) == Crimson;

    [HarmonyPatch]
    private static class Patches
    {
        [HarmonyPatch(
            typeof(CreatureCmd),
            MethodType.Async,
            typeof(PlayerChoiceContext),
            typeof(IEnumerable<Creature>),
            typeof(decimal),
            typeof(ValueProp),
            typeof(Creature),
            typeof(CardModel),
            typeof(CardPlay)
        )]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions
        )
        {
            var method = typeof(Creature).GetMethod(nameof(Creature.LoseHpInternal));
            var firstReplacement = true;
            foreach (var instruction in instructions)
            {
                if (instruction.Calls(method) && firstReplacement)
                {
                    yield return CodeInstruction.Call(
                        (Creature target, decimal unblockedDamage, ValueProp valueProps) =>
                            // ReSharper disable once ConvertClosureToMethodGroup
                            LoseHpInternalWrapper(target, unblockedDamage, valueProps)
                    );
                }
                else
                    yield return instruction;
            }
        }

        private static DamageResult LoseHpInternalWrapper(
            Creature target,
            decimal unblockedDamage,
            ValueProp valueProps
        )
        {
            if (valueProps.IsCrimsonAttack())
                unblockedDamage *= 2;

            return target.LoseHpInternal(unblockedDamage, valueProps);
        }
    }
}
