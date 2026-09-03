using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Combat.History;
using NewKunlun.NewKunlunCode.Powers;

namespace NewKunlun.NewKunlunCode.Extensions;

public static class CreatureExtensions
{
    extension(Creature self)
    {
        public async Task<decimal> ComputeBlockGain(BlockVar block, CardPlay? cardPlay = null)
        {
            var combatState = self.CombatState;
            await Hook.BeforeBlockGained(
                combatState!,
                self,
                block.BaseValue,
                block.Props,
                cardPlay?.Card
            );
            var modifiedAmount = block.BaseValue;
            modifiedAmount = Hook.ModifyBlock(
                combatState!,
                self,
                modifiedAmount,
                block.Props,
                cardPlay?.Card,
                cardPlay,
                out var modifiers
            );
            modifiedAmount = Math.Max(modifiedAmount, 0M);
            await Hook.AfterModifyingBlockAmount(
                combatState!,
                modifiedAmount,
                cardPlay?.Card,
                cardPlay,
                modifiers
            );
            return modifiedAmount;
        }

        public bool HasTalismanFor(Creature applier) =>
            self.GetPowerInstances<TalismanPower>().Any(p => p.Applier == applier);

        public bool HasTalismanFor(Player player) => self.HasTalismanFor(player.Creature);

        public bool HadTalismanThisTurnFor(Creature applier)
        {
            var combatState = self.CombatState;
            return self.HasTalismanFor(applier)
                || combatState is not null
                    && CombatManager
                        .Instance.History.Entries.OfType<TalismanRemovedEntry>()
                        .Any(entry =>
                            entry.Actor == self
                            && entry.Applier == applier
                            && entry.RoundNumber == combatState.RoundNumber
                        );
        }

        public bool HadTalismanThisTurnFor(Player player) =>
            self.HadTalismanThisTurnFor(player.Creature);

        public bool ShouldTriggerFatal() => self.Powers.All(p => p.ShouldOwnerDeathTriggerFatal());
    }
}
