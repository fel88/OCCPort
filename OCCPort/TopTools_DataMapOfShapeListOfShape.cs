using OCCPort.Tester;
using System;

namespace OCCPort
{
    internal class TopTools_DataMapOfShapeListOfShape : NCollection_DataMap<TopoDS_Shape, TopTools_ListOfShape, TopTools_ShapeMapHasher>
    {
        internal bool IsBound(TopoDS_Shape v1)
        {
            return ContainsKey(v1);
        }
    }
}