using MegaCrit.Sts2.Core.Random;

namespace NewKunlun.NewKunlunCode.Extensions;

public static class RngExtensions
{
    extension(Rng self)
    {
        public IEnumerable<T> NextItems<T>(int amount, IEnumerable<T> items)
        {
            switch (amount)
            {
                case 0:
                    return [];
                case 1:
                {
                    var item = self.NextItem(items);
                    return item != null ? [item] : [];
                }
                default:
                {
                    List<T> list = [.. items];
                    self.Shuffle(list);
                    return list.Take(amount);
                }
            }
        }
    }
}
