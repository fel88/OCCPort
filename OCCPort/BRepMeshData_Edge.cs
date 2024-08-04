namespace OCCPort
{
    //! Default implementation of edge data model entity.
    public class BRepMeshData_Edge : IMeshData_Edge
    {
        public BRepMeshData_Edge(TopoDS_Edge theEdge) : base(theEdge)
        {
        }
    }
}