using System;
using System.Collections.Generic;

namespace OCCPort
{
    public class NCollection_DataMap<T1, T2> : Dictionary<T1, T2>
    {
        public bool Bind(T1 aFreeEdgeId, T2 v)
        {
            bool ret = false;
            if (!ContainsKey(aFreeEdgeId))
                ret = true;

            Add(aFreeEdgeId, v);
            return ret;
        }
    }
}