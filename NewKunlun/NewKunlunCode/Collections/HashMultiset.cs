using System.Collections;

namespace NewKunlun.NewKunlunCode.Collections;

public sealed class HashMultiset<T>
    where T : notnull
{
    private readonly Dictionary<T, int> _rep = [];

    public int Count { get; private set; }

    public IReadOnlySet<T> Distinct => new SetView(this);

    public bool Contains(T item, int count = 1) => CountOf(item) >= count;

    public int CountOf(T item) => _rep.GetValueOrDefault(item, 0);

    public IEnumerable<(T, int)> Counts => _rep.Select(kvp => (kvp.Key, kvp.Value));

    public void Add(T item, int count = 1)
    {
        if (count == 0)
            return;
        if (count < 0)
            throw new ArgumentException($"{nameof(count)} must be non-negative.");

        Count += count;
        _rep[item] = CountOf(item) + count;
    }

    // Returns the amount removed.
    public int Remove(T item, int count = 1)
    {
        if (!_rep.TryGetValue(item, out var prev))
            return 0;

        var removed = count > prev ? prev : count;
        Count -= removed;
        if (count == prev)
            _rep.Remove(item);
        else
            _rep[item] = prev - count;
        return removed;
    }

    private class SetView(HashMultiset<T> parent) : IReadOnlySet<T>
    {
        private Dictionary<T, int>.KeyCollection Keys => parent._rep.Keys;

        public IEnumerator<T> GetEnumerator() => Keys.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Keys.GetEnumerator();

        public int Count => parent._rep.Count;

        public bool Contains(T item) => parent._rep.ContainsKey(item);

        private static IReadOnlySet<T> GetSet(IEnumerable<T> other) =>
            other is IReadOnlySet<T> set ? set : new HashSet<T>(other);

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            var set = GetSet(other);
            return Count < set.Count && Keys.All(set.Contains);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            var set = GetSet(other);
            return Count > set.Count && set.All(Contains);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            var set = GetSet(other);
            return set.Count >= Count && Keys.All(set.Contains);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            var set = GetSet(other);
            return Count >= set.Count && set.All(Contains);
        }

        public bool Overlaps(IEnumerable<T> other) => other.Any(Contains);

        public bool SetEquals(IEnumerable<T> other)
        {
            var set = GetSet(other);
            return Count == set.Count && set.All(Contains);
        }
    }
}
