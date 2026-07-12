using OCCPort.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace TKernel
{
    public class NCollection_IndexedMap<T> : NCollection_IndexedMap<T, NCollection_DefaultHasher<T>>
    {

    }

    public class NCollection_IndexedMap<T, Hasher> : List<T> where Hasher : IEqualityComparer<T>, new()
    {
        public class Iterator
        {
            NCollection_IndexedMap<T, Hasher> col;
            public Iterator(NCollection_IndexedMap<T, Hasher> aStructures)
            {
                col = aStructures;
            }

            int index = 0;
            public bool More()
            {
                return index < col.Count;
            }

            public void Next()
            {
                index++;
            }

            public T Value()
            {
                return col[index];
            }
        }
        public NCollection_IndexedMap()
        {

        }

        public void Swap(int theIndex1, int theIndex2)
        {
            Exceptions.Standard_OutOfRange_Raise_if(theIndex1 < 1 || theIndex1 > Extent()
                                  || theIndex2 < 1 || theIndex2 > Extent(), "NCollection_IndexedMap::Swap");

            if (theIndex1 == theIndex2)
                return;

            T tmp = this[theIndex1 - 1];
            this[theIndex1 - 1] = this[theIndex2 - 1];
            this[theIndex2 - 1] = tmp;
        }

        public void RemoveLast()
        {
            RemoveAt(Count - 1);
        }
        public bool IsEmpty()
        {
            return Count == 0;
        }

        //! FindIndex
        public int FindIndex(T theKey1)
        {
            if (IsEmpty())
                return 0;

            for (int i = 0; i < Count; i++)
            {
                if (hasher.Equals(this[i], theKey1))
                {
                    //RemoveAt(i);
                    return i + 1;
                }
            }
            return 0;
        }

        Hasher hasher = new Hasher();

        public void RemoveKey(T v1)
        {
            for (int i = 0; i < Count; i++)
            {
                if (hasher.Equals(this[i], v1))
                {
                    RemoveAt(i);
                    return;
                }
            }
        }

        public new int Add(T theStruct)
        {
            if (this.Contains(theStruct, hasher))
            {
                for (int i = 0; i < Count; i++)
                {
                    if (hasher.Equals(this[i], theStruct))
                        return i + 1;
                }
            }

            base.Add(theStruct);
            return Count;
        }

        public int Size()
        {
            return Count;
        }
        public int Extent()
        {
            return Count;
        }
    }
}
