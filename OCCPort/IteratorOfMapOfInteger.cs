using System;

namespace OCCPort
{
    internal class IteratorOfMapOfInteger
    {

        MapOfInteger list;
        public IteratorOfMapOfInteger(MapOfInteger aTriangles)
        {
            list = aTriangles;
        }

        internal int Key()
        {
            return list[index];
        }
        int index = 0;
        internal bool More()
        {
            return index < list.Count - 1;
        }

        internal void Next()
        {
            index++;
        }
    }
}