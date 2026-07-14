using System.Diagnostics.CodeAnalysis;

namespace TKMesh
{
    public class WeakEqual<T> : IEqualityComparer<T> where T : class
    {
        public bool Equals(T x, T y)
        {
            return x == y;
        }

        public int GetHashCode([DisallowNull] T obj)
        {
            return obj.GetHashCode();
        }
    }
}

