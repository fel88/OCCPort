using System;
using System.Collections.Generic;

namespace OCCPort
{
    /**
 * Optimized Map of integer values. Each block of 32 integers is stored in 8 bytes in memory.
 */
    internal class TColStd_PackedMapOfInteger
    {
        public  void Clear()
        {

        }
        public bool Contains(int d)
        {
            return list.Contains(d);
        }
        HashSet<int> list = new HashSet<int>();
        internal void Add(int mytr)
        {
            list.Add(mytr);
        }
    }
}