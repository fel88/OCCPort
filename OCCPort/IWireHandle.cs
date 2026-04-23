using OCCPort.Interfaces;

namespace OCCPort
{
    public interface IWireHandle : IMeshData_Wire
    {
        int AddEdge(IMeshData_Edge theDEdge, TopAbs_Orientation theOrientation);


    }

}