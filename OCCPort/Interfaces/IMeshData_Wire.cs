using System.Net.NetworkInformation;

namespace OCCPort.Interfaces
{
    //! Interface class representing discrete model of a wire.
    //! Wire should represent an ordered set of edges.
    public interface IMeshData_Wire : IMeshData_TessellatedShape, IMeshData_StatusOwner
    {
        int EdgesNb();
        IEdgeHandle GetEdge(int aEdgeIt);

    }
}