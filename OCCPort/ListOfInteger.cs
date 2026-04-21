using System;
using System.Collections.Generic;

namespace OCCPort
{
    public class ListOfInteger : List<int>
    {
        public ListOfInteger() { }
        public ListOfInteger(object myAllocator)
        {
        }

        internal int First()
        {
            return this[0];
        }
        public bool IsEmpty()
        {
            return Count == 0;
        }

        internal void RemoveFirst()
        {
            RemoveAt(0);
        }

        internal int Last()
        {
            return this[^1];
        }
    }
}