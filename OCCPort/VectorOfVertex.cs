using System.Collections.Generic;

namespace OCCPort
{
    internal class VectorOfVertex : List<BRepMesh_Vertex>
    {
        public int Length()
        {
            return Count;
        }
    }
}