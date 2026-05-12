using System;

namespace OCCPort
{
    internal class TopTools_IndexedMapOfShape : NCollection_IndexedMap<TopoDS_Shape, TopTools_ShapeMapHasher>
    {
        internal int Extent()
        {
            return Count;
        }
        public bool IsEmpty()
        {
            return Count == 0;
        }

        internal TopoDS_Shape FindKey(int i)
        {            
            return this[i - 1];
        }

    }
}