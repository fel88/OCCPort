using System;
using TKBRep;
using TKernel;

namespace OCCPort
{
    public class TopTools_MapOfShape : NCollection_Map<TopoDS_Shape>//NCollection_Map<TopoDS_Shape,TopTools_ShapeMapHasher> 
    {
        public int Extent()
        {
            return Count;
        }
    }
}