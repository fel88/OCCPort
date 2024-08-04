using System.Collections.Generic;

namespace OCCPort
{
    public class VectorOfIFaceHandles
    {
        //  IMeshData_Face
        List<IMeshData_Face> items = new List<IMeshData_Face>();
        public int Size()
        {
            return items.Count;
        }

        internal void Append(IMeshData_Face aEdge)
        {
            items.Add(aEdge);
        }

        internal IMeshData_Face Get(int v)
        {
            return items[v];
        }
    }
}