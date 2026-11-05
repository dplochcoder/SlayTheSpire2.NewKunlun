using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NewKunlun.NewKunlunCode.Collections;

[CollectionBuilder(typeof(BimapBuilder), nameof(BimapBuilder.Create))]
public interface IReadOnlyBimap<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    IReadOnlyBimap<TValue, TKey> Inverse { get; }
}

[CollectionBuilder(typeof(BimapBuilder), nameof(BimapBuilder.Create))]
public interface IBimap<TKey, TValue> : IReadOnlyBimap<TKey, TValue>, IDictionary<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    new IBimap<TValue, TKey> Inverse { get; }
}

public sealed class Bimap<TKey, TValue> : IBimap<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    private readonly Dictionary<TKey, TValue> _forward;
    private readonly Dictionary<TValue, TKey> _backward;
    private readonly Bimap<TValue, TKey> _inverse;

    private Bimap(
        Dictionary<TKey, TValue> forward,
        Dictionary<TValue, TKey> backward,
        Bimap<TValue, TKey> inverse
    )
    {
        _forward = forward;
        _backward = backward;
        _inverse = inverse;
    }

    public Bimap()
    {
        _forward = [];
        _backward = [];
        _inverse = new(_backward, _forward, this);
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _forward.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _forward.GetEnumerator();

    private void Set(TKey key, TValue value)
    {
        if (_forward.Remove(key, out var oldValue))
            _backward.Remove(oldValue);
        if (_backward.Remove(value, out var oldKey))
            _forward.Remove(oldKey);

        _forward[key] = value;
        _backward[value] = key;
    }

    public void Add(KeyValuePair<TKey, TValue> item) => Set(item.Key, item.Value);

    public void Clear()
    {
        _forward.Clear();
        _backward.Clear();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item) => _forward.Contains(item);

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) =>
        ((ICollection<KeyValuePair<TKey, TValue>>)_forward).CopyTo(array, arrayIndex);

    public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key, item.Value);

    public int Count => _forward.Count;

    public bool IsReadOnly => false;

    public void Add(TKey key, TValue value) => Set(key, value);

    public void AddStrict(TKey key, TValue value)
    {
        if (ContainsKey(key) || Values.Contains(value))
            throw new ArgumentException($"Duplicate key ({key}) or value ({value}).");

        _forward[key] = value;
        _backward[value] = key;
    }

    public bool ContainsKey(TKey key) => _forward.ContainsKey(key);

    public bool Remove(TKey key, TValue value)
    {
        if (
            !_forward.TryGetValue(key, out var oldValue)
            || !EqualityComparer<TValue>.Default.Equals(oldValue, value)
        )
            return false;

        _forward.Remove(key);
        _backward.Remove(value);
        return true;
    }

    public bool Remove(TKey key)
    {
        if (!_forward.Remove(key, out var oldValue))
            return false;

        _backward.Remove(oldValue);
        return true;
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) =>
        _forward.TryGetValue(key, out value);

    public TValue this[TKey key]
    {
        get =>
            _forward.TryGetValue(key, out var value)
                ? value
                : throw new KeyNotFoundException($"{key}");
        set => Set(key, value);
    }

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

    public ICollection<TKey> Keys => _forward.Keys;

    public ICollection<TValue> Values => _backward.Keys;

    IReadOnlyBimap<TValue, TKey> IReadOnlyBimap<TKey, TValue>.Inverse => _inverse;

    public IBimap<TValue, TKey> Inverse => _inverse;
}

public static class BimapBuilder
{
    public static IBimap<TKey, TValue> Create<TKey, TValue>(
        ReadOnlySpan<KeyValuePair<TKey, TValue>> span
    )
        where TKey : notnull
        where TValue : notnull
    {
        Bimap<TKey, TValue> map = [];
        foreach (var pair in span)
            map.AddStrict(pair.Key, pair.Value);
        return map;
    }
}
