using TKG3d;

namespace OCCPort
{
    public abstract class TopoDS_TEdge : TopoDS_TShape
    {
      

        public override TopAbs_ShapeEnum ShapeType()
        {
            return TopAbs_ShapeEnum.TopAbs_EDGE;
        }
    }
}