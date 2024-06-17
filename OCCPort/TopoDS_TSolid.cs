namespace OCCPort
{

    //! A Topological part of 3D space, bounded by shells,
    //! edges and vertices.
    public class TopoDS_TSolid : TopoDS_TShape
    {
        public override TopAbs_ShapeEnum ShapeType()
        {
            return TopAbs_ShapeEnum.TopAbs_SOLID;
        }
    }
}