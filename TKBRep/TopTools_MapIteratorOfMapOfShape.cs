using System;
using TKBRep;
using TKernel;

namespace OCCPort
{
    public class TopTools_MapIteratorOfMapOfShape : NCollection_Map<TopoDS_Shape, TopTools_ShapeMapHasher>.Iterator
    {
        public TopTools_MapIteratorOfMapOfShape(NCollection_Map<TopoDS_Shape, TopTools_ShapeMapHasher> theMap) : base(theMap)
        {
        }
    }
}