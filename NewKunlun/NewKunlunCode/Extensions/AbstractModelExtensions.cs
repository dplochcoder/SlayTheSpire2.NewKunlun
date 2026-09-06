using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Extensions;

public static class AbstractModelExtensions
{
    extension(AbstractModel self)
    {
        public CardNameVar<T> MakeCardNameVar<T>(Func<bool> upgraded)
            where T : CardModel => new CardNameVar<T>(upgraded);
    }
}
