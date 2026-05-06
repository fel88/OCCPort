using OCCPort.Tester;
using System;

namespace OCCPort
{
    internal class TopTools_DataMapOfShapeListOfShape : NCollection_DataMap<TopoDS_Shape, TopTools_ListOfShape, TopTools_ShapeMapHasher>
    {
        internal bool IsBound(TopoDS_Shape v1)
        {
          /*  foreach (var item in Keys)
            {
                if (TopTools_ShapeMapHasher.IsEqual(v1, item))
                    return true;
            }
            return false;*/
            return ContainsKey(v1);
        }
    }
}