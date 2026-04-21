using OCCPort.Interfaces;

namespace OCCPort
{
    //! Default implementation of edge data model entity.
    public class BRepMeshData_Wire : AbstractMeshData_TessellatedShape, IMeshData_Wire
    {
        public int EdgesNb()
        {
            return myDEdges.Count;
        }

        public IEdgeHandle GetEdge(int aEdgeIt)
        {
            return myDEdges[aEdgeIt];
        }

        VectorOfIEdgePtrs myDEdges;
        VectorOfOrientation myDEdgesOri;

        public BRepMeshData_Wire(TopoDS_Shape theShape) : base(theShape)
        {
        }


    }
}