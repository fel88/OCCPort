using System;
using System.Collections.Generic;
using static OCCPort.IMeshData;

namespace OCCPort
{
    internal class VectorOfIEdgeHandles
    {
        List<IMeshData_Edge> items = new List<IMeshData_Edge>();
        public int Size()
        {
            return items.Count;
        }

        internal void Append(IMeshData_Edge aEdge)
        {
            items.Add(aEdge);
        }

        internal IMeshData_Edge Get(int v)
        {
            return items[v];
        }
    }
}