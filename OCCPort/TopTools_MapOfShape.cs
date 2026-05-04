using System;

namespace OCCPort
{
    internal class TopTools_MapOfShape : NCollection_Map<TopoDS_Shape>//NCollection_Map<TopoDS_Shape,TopTools_ShapeMapHasher> 
    {
        internal int Extent()
        {
            return Count;
        }
    }
}