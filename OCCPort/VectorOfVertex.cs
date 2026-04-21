using System;
using System.Collections.Generic;

namespace OCCPort
{
    internal class VectorOfVertex : List<BRepMesh_Vertex>
    {
        public int Length()
        {
            return Count;
        }
        public BRepMesh_Vertex Value(int index)
        {
            return this[index];
        }

        internal void Append(BRepMesh_Vertex theVertex)
        {
            Add(theVertex);
        }

        internal void ChangeValue(int v, BRepMesh_Vertex theVertex)
        {
            this[v] = theVertex;
        }
    }
}