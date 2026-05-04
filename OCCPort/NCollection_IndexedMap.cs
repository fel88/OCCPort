using System.Collections.Generic;

namespace OCCPort
{
    public class NCollection_IndexedMap<T, Hasher> : List<T>
    {
        internal new int Add(T theStruct)
        {
            base.Add(theStruct);
            return Count;
        }

        internal int Size()
        {
            return Count;
        }

    }
}