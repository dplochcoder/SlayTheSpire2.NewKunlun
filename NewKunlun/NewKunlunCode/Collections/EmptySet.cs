using System.Collections;

namespace NewKunlun.NewKunlunCode.Collections;

public sealed class EmptySet<T> : IReadOnlySet<T>
{
    private EmptySet() { }

    public static readonly EmptySet<T> Instance = new();

    public IEnumerator<T> GetEnumerator() => EmptyEnumerator<T>.Instance;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => 0;

    public bool Contains(T item) => false;

    public bool IsProperSubsetOf(IEnumerable<T> other) => other.Any();

    public bool IsProperSupersetOf(IEnumerable<T> other) => false;

    public bool IsSubsetOf(IEnumerable<T> other) => true;

    public bool IsSupersetOf(IEnumerable<T> other) => !other.Take(1).Any();

    public bool Overlaps(IEnumerable<T> other) => false;

    public bool SetEquals(IEnumerable<T> other) => !other.Take(1).Any();
}
