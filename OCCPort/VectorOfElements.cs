using System;
using System.Collections.Generic;

namespace OCCPort
{
    public class VectorOfElements : List<BRepMesh_Triangle>
    {
        public VectorOfElements(int capacity, NCollection_IncAllocator myAllocator) : base(capacity)
        {
        }

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