using TKG3d;

namespace OCCPort
{
    internal abstract class TopoDS_TEdge : TopoDS_TShape
    {
      

        public override TopAbs_ShapeEnum ShapeType()
        {
            return TopAbs_ShapeEnum.TopAbs_EDGE;
        }
    }
}