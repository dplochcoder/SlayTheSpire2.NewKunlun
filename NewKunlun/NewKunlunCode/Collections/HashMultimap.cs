using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace NewKunlun.NewKunlunCode.Collections;

public sealed class HashMultimap<TKey, TValue> : IReadOnlyDictionary<TKey, IReadOnlySet<TValue>>
    where TKey : notnull
{
    private readonly Dictionary<TKey, HashSet<TValue>> _rep = [];

    public int Count { get; private set; }

    public IEnumerator<KeyValuePair<TKey, IReadOnlySet<TValue>>> GetEnumerator() =>
        _rep.Select(kvp => new KeyValuePair<TKey, IReadOnlySet<TValue>>(kvp.Key, kvp.Value))
            .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool ContainsKey(TKey key) => _rep.ContainsKey(key);

    public bool Contains(TKey key, TValue value) =>
        TryGetValue(key, out var set) && set.Contains(value);

    public bool Add(TKey key, TValue value) => Add(key, [value]);

    public bool Add(TKey key, IEnumerable<TValue> values)
    {
        if (!_rep.TryGetValue(key, out var set))
        {
            set = [];
            _rep[key] = set;
        }

        var modified = false;
        foreach (var value in values)
        {
            if (!set.Add(value))
                continue;

            modified = true;
            ++Count;
        }

        return modified;
    }

    public bool Remove(TKey key, TValue value)
    {
        if (!_rep.TryGetValue(key, out var set) || !set.Remove(value))
            return false;
        if (set.Count == 0)
            _rep.Remove(key);

        --Count;
        return true;
    }

    private bool RemoveInternal(TKey key, out IReadOnlySet<TValue> values)
    {
        if (!_rep.TryGetValue(key, out var set))
        {
            values = EmptySet<TValue>.Instance;
            return false;
        }

        Count -= set.Count;
        _rep.Remove(key);
        values = set;
        return true;
    }

    public bool Remove(TKey key) => RemoveInternal(key, out _);

    public IReadOnlySet<TValue> TakeValues(TKey key) =>
        RemoveInternal(key, out var set) ? set : EmptySet<TValue>.Instance;

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out IReadOnlySet<TValue> value)
    {
        if (_rep.TryGetValue(key, out var set))
        {
            value = set;
            return true;
        }

        value = EmptySet<TValue>.Instance;
        return false;
    }

    public IReadOnlySet<TValue> this[TKey key] =>
        _rep.TryGetValue(key, out var set) ? set : EmptySet<TValue>.Instance;

    public IEnumerable<TKey> Keys => _rep.Keys;
    public IEnumerable<IReadOnlySet<TValue>> Values => _rep.Values;
    public IEnumerable<TValue> AllValues => Values.SelectMany(s => s);
}
