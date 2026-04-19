using OCCPort.Interfaces;

namespace OCCPort
{
    //! Default implementation of edge data model entity.
    public class BRepMeshData_Edge : AbstractMeshData_TessellatedShape, IMeshData_Edge
    {
        public BRepMeshData_Edge(TopoDS_Edge theEdge) : base(theEdge)
        {
        }
    }
}