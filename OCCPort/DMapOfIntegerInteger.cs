using OpenTK.Core.Native;
using System;
using System.Collections.Generic;
using TKernel;

namespace OCCPort
{
    internal class DMapOfIntegerInteger : NCollection_DataMap<int, int, NCollection_DefaultHasher<int>>
    {
        Dictionary<int, int> map = new Dictionary<int, int>();
        public DMapOfIntegerInteger(int v, NCollection_IncAllocator myAllocator)
        {
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