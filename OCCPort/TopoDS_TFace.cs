namespace OCCPort
{
	//! A  topological part  of a surface   or  of the  2D
	//! space.  The  boundary  is  a   set of  wires   and
	//! vertices.

	public class TopoDS_TFace : TopoDS_TShape

    {
        public override TopAbs_ShapeEnum ShapeType()
        {
            return TopAbs_ShapeEnum.TopAbs_FACE;
        }
    }
}