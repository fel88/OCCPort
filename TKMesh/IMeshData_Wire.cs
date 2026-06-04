using TKG3d;

namespace TKMesh
{
    //! Interface class representing discrete model of a wire.
    //! Wire should represent an ordered set of edges.
    public interface IMeshData_Wire : IMeshData_TessellatedShape, IMeshData_StatusOwner
    {
        int AddEdge(IMeshData_Edge theDEdge, TopAbs_Orientation theOrientation);

        int EdgesNb();
        IMeshData_Edge GetEdge(int aEdgeIt);
        TopAbs_Orientation GetEdgeOrientation(int aEdgeIt);

    }
}

