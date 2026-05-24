using System;
using System.Collections.Generic;

namespace OCCPort
{
    public class NCollection_List<T> : List<T>
    {
        public void Append(T aPoints)
        {
            Add(aPoints);
        }

        public T First()
        {
            return this[0];
        }
        public T Last()
        {
            return this[^1];
        }

        public bool IsEmpty()
        {
            return Count == 0;
        }
        public int Size()
        {
            return Count;
        }
    }
}