using OCCPort.Interfaces;

namespace OCCPort
{
    public interface IWireHandle : IMeshData_Wire
    {
        TopAbs_Orientation GetEdgeOrientation(int aEdgeIt);
    }

}