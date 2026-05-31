using System;
using System.Linq;

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

        public class Iterator
        {
            MapOfIntegerInteger poly;
            public Iterator(MapOfIntegerInteger thePoly)
            {
                poly = thePoly;
            }

            int index = 0;
            internal bool More()
            {
                return index < poly.Count;
            }

            internal void Next()
            {
                index++;
            }

            internal int Key()
            {
                var ordKeys = poly.Keys.OrderBy(z => z).ToArray();
                return ordKeys[index];
            }

            internal void Initialize(MapOfIntegerInteger thePoly)
            {
                poly = thePoly;
                index = 0;
            }
        }
    }
}