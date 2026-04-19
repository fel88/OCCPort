using System;
using System.Collections.Generic;

namespace OCCPort
{
    public class VectorOfElements : List<BRepMesh_Triangle>
    {
        internal void Append(BRepMesh_Triangle theElement)
        {
            Add(theElement);
        }

        internal int Size()
        {
            return Count;
        }
    }
}