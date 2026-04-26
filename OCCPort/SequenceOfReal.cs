using System;
using System.Collections.Generic;

namespace OCCPort
{
    internal class SequenceOfReal : List<double>
    {
        internal void erase(int v1, int v2)
        {
            RemoveRange(v1, v2);
        }

        internal int size()
        {
            return Count;
        }
    }
}