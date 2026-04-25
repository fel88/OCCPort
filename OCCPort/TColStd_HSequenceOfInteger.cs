using System;
using System.Collections.Generic;

namespace OCCPort
{
    internal class TColStd_HSequenceOfInteger : List<int>
    {
        internal void Append(int v)
        {
            Add(v);
        }
        internal void Prepend(int v)
        {
            Insert(0, v);
        }
    }
}