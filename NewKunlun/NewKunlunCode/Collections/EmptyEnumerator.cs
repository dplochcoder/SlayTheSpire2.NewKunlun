using System.Collections;

namespace NewKunlun.NewKunlunCode.Collections;

public sealed class EmptyEnumerator<T> : IEnumerator<T>
{
    private EmptyEnumerator() { }

    public static readonly EmptyEnumerator<T> Instance = new();

    public bool MoveNext() => false;

    public void Reset() { }

    T IEnumerator<T>.Current => throw new NotSupportedException("Current");

    object? IEnumerator.Current => throw new NotSupportedException("Current");

    public void Dispose() { }
}
