using OCCPort.Common;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace TKernel
{
    public class NCollection_DefaultHasher<T> : IHasher<T>
    {
        public bool Equals(T? x, T? y)
        {
            return x.Equals(y);
        }

        public int GetHashCode([DisallowNull] T obj)
        {
            return obj.GetHashCode();
        }

        public int HashCode(T theValue, int theUpperBound)
        {
            int hash = 0;
            if (theValue is IHashCode hashCode)
            {
                hash = hashCode.HashCode(theUpperBound);
            }
            else { hash = theValue.GetHashCode(); }
            var mask = Standard_Integer.IntegerLast();
            //return static_cast<Standard_Integer> ((theValue & theMask) % theUpperBound + 1);
            return (hash & mask) % theUpperBound + 1;
        }

        public bool IsEqual(T theKeyType, T theKey1)
        {
            return theKeyType.Equals(theKey1);
        }
    }
}