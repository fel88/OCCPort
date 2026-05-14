using System;

namespace OCCPort
{
    internal class MapOfIntegerInteger : NCollection_DataMap<int, int, NCollection_DefaultHasher<int>>
    {
        
        public MapOfIntegerInteger(int v)
        {
        }

        internal bool IsBound(int v)
        {
            return ContainsKey(v);
        }

        internal void UnBind(int v)
        {
            Remove(v);
        }
    }
}