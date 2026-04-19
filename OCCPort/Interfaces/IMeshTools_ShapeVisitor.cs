namespace OCCPort.Interfaces
{
    //! Interface class for shape visitor.
    public interface IMeshTools_ShapeVisitor
    {

        //! Handles TopoDS_Face object.
        void Visit(TopoDS_Face theFace);

        //! Handles TopoDS_Edge object.
        void Visit(TopoDS_Edge theEdge);
    }
}