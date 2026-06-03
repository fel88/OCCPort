namespace OCCPort
{

    //! A Topological part of 3D space, bounded by shells,
    //! edges and vertices.
    public class TopoDS_TSolid : TopoDS_TShape
    {
        public override TopoDS_TShape EmptyCopy()
        {
            throw new System.NotImplementedException();
        }

        public override TopAbs_ShapeEnum ShapeType()
        {
            return TopAbs_ShapeEnum.TopAbs_SOLID;
        }
    }
}