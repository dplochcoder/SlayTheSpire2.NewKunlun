using System.Runtime.CompilerServices;

namespace NewKunlun.NewKunlunCode.Collections;

public class ExtensionField<TKey, TValue>
    where TKey : class
    where TValue : struct
{
    private class Wrapper(TValue value)
    {
        public TValue Value => value;
    }

    private readonly ConditionalWeakTable<TKey, Wrapper> _table = new();

    public bool Get(TKey key, out TValue value)
    {
        if (_table.TryGetValue(key, out var wrapper))
        {
            value = wrapper.Value;
            return true;
        }

        value = default;
        return false;
    }

    public void Set(TKey key, TValue value) => _table.AddOrUpdate(key, new(value));
}
