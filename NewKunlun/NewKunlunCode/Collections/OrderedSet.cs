using System.Collections;

namespace NewKunlun.NewKunlunCode.Collections;

public sealed class OrderedSet<T> : ISet<T>, IReadOnlyList<T>
    where T : notnull
{
    private readonly List<T> _list = [];
    private readonly HashSet<T> _set = [];

    private void UpdateList()
    {
        if (_set.Count == _list.Count)
            return;

        List<T> copy = [.. _list.Where(_set.Contains)];
        _list.Clear();
        _list.AddRange(copy);
    }

    public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();

    public bool Add(T item)
    {
        if (!_set.Add(item))
            return false;

        _list.Add(item);
        return true;
    }

    void ICollection<T>.Add(T item)
    {
        if (_set.Add(item))
            _list.Add(item);
    }

    public void ExceptWith(IEnumerable<T> other)
    {
        _set.ExceptWith(other);
        UpdateList();
    }

    public void IntersectWith(IEnumerable<T> other)
    {
        _set.IntersectWith(other);
        UpdateList();
    }

    public bool IsProperSubsetOf(IEnumerable<T> other) => _set.IsProperSubsetOf(other);

    public bool IsProperSupersetOf(IEnumerable<T> other) => _set.IsProperSupersetOf(other);

    public bool IsSubsetOf(IEnumerable<T> other) => _set.IsSubsetOf(other);

    public bool IsSupersetOf(IEnumerable<T> other) => _set.IsSupersetOf(other);

    public bool Overlaps(IEnumerable<T> other) => _set.Overlaps(other);

    public bool SetEquals(IEnumerable<T> other) => _set.SetEquals(other);

    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        _set.SymmetricExceptWith(other);
        UpdateList();
    }

    public void UnionWith(IEnumerable<T> other)
    {
        foreach (var item in other)
            ((ICollection<T>)this).Add(item);
    }

    public void Clear()
    {
        _set.Clear();
        _list.Clear();
    }

    public bool Contains(T item) => _set.Contains(item);

    public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

    public bool Remove(T item)
    {
        if (!_set.Remove(item))
            return false;

        _list.Remove(item);
        return true;
    }

    int ICollection<T>.Count => _list.Count;

    public bool IsReadOnly => false;

    int IReadOnlyCollection<T>.Count => _list.Count;

    public T this[int index] => _list[index];
}
