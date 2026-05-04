using System;

namespace OCCPort
{
    internal class TopTools_IndexedMapOfShape : NCollection_IndexedMap<TopoDS_Shape, TopTools_ShapeMapHasher>
    {
        internal int Extent()
        {
            return Count;
        }

        internal TopoDS_Shape FindKey(int i)
        {
            return this[i - 1];
        }
    }
}