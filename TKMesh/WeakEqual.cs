using OCCPort.Common;
using System.Diagnostics.CodeAnalysis;
using TKernel;

namespace TKMesh
{
    public class WeakEqual<T> : IHasher<T> where T : class
    {
        public bool Equals(T x, T y)
        {
            return x == y;
        }

        public int GetHashCode([DisallowNull] T obj)
        {
            return obj.GetHashCode();
        }

        public int HashCode(T theValue, int theUpperBound)
        {
            return Standard_Integer.HashCode(theValue.GetHashCode(), theUpperBound);
        }

        public bool IsEqual(T theKeyType, T theKey1)
        {
            return theKeyType == theKey1;
        }
    }
}

