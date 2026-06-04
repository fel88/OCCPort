using System;
using TKernel;

namespace OCCPort
{
    public class TopTools_IndexedMapOfShape : NCollection_IndexedMap<TopoDS_Shape, TopTools_ShapeMapHasher>
    {
        public int Extent()
        {
            return Count;
        }
        public bool IsEmpty()
        {
            return Count == 0;
        }

        public TopoDS_Shape FindKey(int i)
        {            
            return this[i - 1];
        }

    }
}