using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

public class WeakEqual<T> : IEqualityComparer<T>
{
    public bool Equals(T? x, T? y)
    {
        throw new System.NotImplementedException();
    }

    public int GetHashCode([DisallowNull] T obj)
    {
        throw new System.NotImplementedException();
    }
}