using System.Collections.Generic;

namespace OCCPort
{
    internal class VectorOfIWireHandles
    {
        List<IMeshData_Wire> items = new List<IMeshData_Wire>();
        public int Size()
        {
            return items.Count;
        }

        internal void Append(IMeshData_Wire aEdge)
        {
            items.Add(aEdge);
        }

        internal IMeshData_Wire Get(int v)
        {
            return items[v];
        }
    }
}