using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace NewKunlun.NewKunlunCode.Extensions;

public static class AttackCommandExtensions
{
    extension(AttackCommand self)
    {
        public AttackCommand WithHeavySlashVfx() => self.WithSlashVfx(heavy: true);

        public AttackCommand WithSlashVfx(bool heavy = false) =>
            heavy
                ? self.WithHitFx("vfx/vfx_heavy_blunt", tmpSfx: "heavy_attack.mp3")
                : self.WithHitFx("vfx/vfx_attack_slash");

        public AttackCommand WithHyperbeamVfx(Player attacker) =>
            self.WithAttackerAnim("Cast", 0.5f).BeforeDamage(() => SpawnHyperbeamVfx(attacker));
    }

    private static async Task SpawnHyperbeamVfx(Player attacker)
    {
        var combatState = attacker.Creature.CombatState;
        if (combatState == null)
            return;

        List<Creature> enemies = [.. combatState.Enemies.Where(c => c.IsAlive)];
        if (NHyperbeamVfx.Create(attacker.Creature, enemies.Last()) is { } node1)
        {
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(node1);
            await Cmd.Wait(0.5f);
        }

        foreach (var enemy in enemies)
        {
            if (NHyperbeamImpactVfx.Create(attacker.Creature, enemy) is { } node2)
                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(node2);
        }
    }
}
