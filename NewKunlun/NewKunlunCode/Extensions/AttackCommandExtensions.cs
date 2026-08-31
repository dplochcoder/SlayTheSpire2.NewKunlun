using MegaCrit.Sts2.Core.Commands.Builders;

namespace NewKunlun.NewKunlunCode.Extensions;

public static class AttackCommandExtensions
{
    public static AttackCommand WithHeavySlashVfx(this AttackCommand self) =>
        self.WithHitFx("vfx/vfx_heavy_blunt", tmpSfx: "heavy_attack.mp3");

    public static AttackCommand WithSlashVfx(this AttackCommand self) =>
        self.WithHitFx("vfx/vfx_attack_slash");
}
