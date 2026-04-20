using OCCPort.Interfaces;
using System.Collections.Generic;

namespace OCCPort
{
    internal class VectorOfIWireHandles
    {
        List<IWireHandle> items = new List<IWireHandle>();
        public int Size()
        {
            return items.Count;
        }

        internal void Append(IWireHandle aEdge)
        {
            items.Add(aEdge);
        }

        internal IWireHandle Get(int v)
        {
            return items[v];
        }
    }
}