namespace OCCPort
{
    internal class TopoDS_TShell : TopoDS_TShape
    {
        public override TopAbs_ShapeEnum ShapeType()
        {
            return TopAbs_ShapeEnum.TopAbs_SHELL;
        }
    }
}