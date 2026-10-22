using MegaCrit.Sts2.Core.Models;

namespace NewKunlun.NewKunlunCode.Extensions;

public static class RelicExtensions
{
    extension<T>(T self)
        where T : RelicModel
    {
        public T? NextUnused => self.Owner.Relics.OfType<T>().FirstOrDefault(r => !r.IsUsedUp);

        public bool IsNextUnused => self == self.NextUnused;
    }
}
