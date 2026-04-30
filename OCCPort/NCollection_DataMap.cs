using System;
using System.Collections.Generic;

namespace OCCPort
{
    internal class NCollection_DataMap<T1, T2>:Dictionary<T1,T2>
    {
        internal void Bind(T1 aFreeEdgeId, T2 v)
        {
            Add(aFreeEdgeId, v);
        }
    }
}