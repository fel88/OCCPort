using System;
using System.Collections.Generic;

namespace OCCPort
{
    public class NCollection_DataMap<T1, T2, T3> : Dictionary<T1, T2> where T3 : IEqualityComparer<T1>, new()
    {

        public NCollection_DataMap() : base(new T3())
        {

        }

        public bool Bind(T1 aFreeEdgeId, T2 v)
        {
            bool ret = false;
            if (!ContainsKey(aFreeEdgeId))
            {
                ret = true;
                Add(aFreeEdgeId, v);
            }
            else
            {
                this[aFreeEdgeId] = v;
            }
            return ret;
        }
    }
}