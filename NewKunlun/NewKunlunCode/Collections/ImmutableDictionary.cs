using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NewKunlun.NewKunlunCode.Collections;

[CollectionBuilder(typeof(ImmutableDictionaryBuilder), nameof(ImmutableDictionaryBuilder.Create))]
public class ImmutableDictionary<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> rep)
    : IReadOnlyDictionary<TKey, TValue>
    where TKey : notnull
{
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => rep.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => rep.GetEnumerator();

    public int Count => rep.Count;

    public bool ContainsKey(TKey key) => rep.ContainsKey(key);

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) =>
        rep.TryGetValue(key, out value);

    public TValue this[TKey key] => rep[key];

    public IEnumerable<TKey> Keys => rep.Keys;

    public IEnumerable<TValue> Values => rep.Values;
}

public static class ImmutableDictionaryBuilder
{
    public static ImmutableDictionary<TKey, TValue> Create<TKey, TValue>(
        ReadOnlySpan<KeyValuePair<TKey, TValue>> span
    )
        where TKey : notnull
    {
        Dictionary<TKey, TValue> dict = [];
        foreach (var pair in span)
            dict.Add(pair.Key, pair.Value);
        return new(dict);
    }
}
