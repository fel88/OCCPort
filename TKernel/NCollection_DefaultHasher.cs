using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace TKernel
{
    public class NCollection_DefaultHasher<T> : IEqualityComparer<T>
    {
        public bool Equals(T? x, T? y)
        {
            return x.Equals(y);
        }

        public int GetHashCode([DisallowNull] T obj)
        {
            return obj.GetHashCode();
        }
    }
}