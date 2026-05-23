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