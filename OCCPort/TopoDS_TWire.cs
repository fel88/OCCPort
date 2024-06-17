namespace OCCPort
{
	//! A set of edges connected by their vertices.

	public class TopoDS_TWire: TopoDS_TShape
    {
        public override TopAbs_ShapeEnum ShapeType()
        {
            return TopAbs_ShapeEnum.TopAbs_WIRE;
        }
    }
}