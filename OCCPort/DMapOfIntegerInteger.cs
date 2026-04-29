using OpenTK.Core.Native;
using System;
using System.Collections.Generic;

namespace OCCPort
{
    internal class DMapOfIntegerInteger : NCollection_DataMap<int, int>
    {
        Dictionary<int, int> map = new Dictionary<int, int>();
        public DMapOfIntegerInteger(int v, NCollection_IncAllocator myAllocator)
        {
        }

        internal void Bind(int v1, int v2)
        {
            map.Add(v1, v2);
        }

        internal int Extent()
        {
            return map.Count;
        }

        internal int Find(int v)
        {
            return map[v];
        }

        internal bool IsBound(int v)
        {
            return map.ContainsKey(v);
        }

        internal int Size()
        {
            return map.Count;
        }
    }
}