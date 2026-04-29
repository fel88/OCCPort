using System;
using System.Collections.Generic;

namespace OCCPort
{
    public class MapOfInteger:List<int>
    {
        public bool IsEmpty()
        {
            return Count == 0;
        }

        internal int Extent()
        {
            return Count;
        }
    }
}