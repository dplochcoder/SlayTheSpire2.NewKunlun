using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Powers;

namespace NewKunlun.NewKunlunCode.Extensions;

public static class CreatureExtensions
{
    extension(Creature self)
    {
        public async Task<decimal> ComputeBlockGain(BlockVar block, CardPlay? cardPlay = null)
        {
            var combatState = self.CombatState;
            if (combatState == null)
                return block.BaseValue;

            await Hook.BeforeBlockGained(
                combatState,
                self,
                block.BaseValue,
                block.Props,
                cardPlay?.Card
            );
            var modifiedAmount = block.BaseValue;
            modifiedAmount = Hook.ModifyBlock(
                combatState,
                self,
                modifiedAmount,
                block.Props,
                cardPlay?.Card,
                cardPlay,
                out var modifiers
            );
            modifiedAmount = Math.Max(modifiedAmount, 0M);
            await Hook.AfterModifyingBlockAmount(
                combatState,
                modifiedAmount,
                cardPlay?.Card,
                cardPlay,
                modifiers
            );
            return modifiedAmount;
        }

        public bool HasTalismanFor(Player player) =>
            self.GetPowerInstances<TalismanPower>().Any(p => p.Applier == player.Creature);

        public bool ShouldTriggerFatal() => self.Powers.All(p => p.ShouldOwnerDeathTriggerFatal());
    }
}
