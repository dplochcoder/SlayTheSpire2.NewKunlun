using MegaCrit.Sts2.Core.Commands.Builders;

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
    }
}
