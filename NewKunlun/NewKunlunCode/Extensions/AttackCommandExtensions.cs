using MegaCrit.Sts2.Core.Commands.Builders;

namespace NewKunlun.NewKunlunCode.Extensions;

public static class AttackCommandExtensions
{
    public static AttackCommand WithSlashFx(this AttackCommand self) =>
        self.WithHitFx("vfx/vfx_attack_slash");
}
