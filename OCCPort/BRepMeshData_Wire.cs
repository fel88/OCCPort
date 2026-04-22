using OCCPort.Interfaces;

namespace OCCPort
{
    //! Default implementation of edge data model entity.
    public class BRepMeshData_Wire : AbstractMeshData_Wire, IWireHandle
    {
        public override  int EdgesNb()
        {
            return myDEdges.Count;
        }

        public override IEdgeHandle GetEdge(int aEdgeIt)
        {
            return myDEdges[aEdgeIt];
        }
        //! Returns True if orientation of discrete edge with the given index is forward.

        public override  TopAbs_Orientation GetEdgeOrientation(int theIndex)
        {
            return myDEdgesOri[theIndex];
        }

        VectorOfIEdgePtrs myDEdges;
        VectorOfOrientation myDEdgesOri;

        public BRepMeshData_Wire(TopoDS_Shape theShape) : base(theShape)
        {
        }

        public BRepMeshData_Wire(TopoDS_Shape theShape, int theEdgeNb, NCollection_IncAllocator myAllocator) : this(theShape)
        {
        }
    }
}