namespace OCCPort
{
    //! A  Vertex is a topological  point in  two or three
    //! dimensions.

    public class TopoDS_TVertex : TopoDS_TShape
    {
        public override TopAbs_ShapeEnum ShapeType()
        {
            return TopAbs_ShapeEnum.TopAbs_VERTEX;
        }
    }
}