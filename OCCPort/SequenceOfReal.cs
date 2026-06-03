using System;
using TKernel;

namespace OCCPort
{
    internal class SequenceOfReal : NCollection_Sequence<double>
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