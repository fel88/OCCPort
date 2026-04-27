namespace OCCPort
{
    internal class TopoDS_TEdge : TopoDS_TShape
    {
        public override TopoDS_TShape EmptyCopy()
        {
            throw new System.NotImplementedException();
        }

        public override TopAbs_ShapeEnum ShapeType()
        {
            return TopAbs_ShapeEnum.TopAbs_EDGE;
        }
    }
}