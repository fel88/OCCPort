namespace OCCPort.Interfaces
{
    //! Interface class representing discrete model of a wire.
    //! Wire should represent an ordered set of edges.
    public interface IMeshData_Wire : IMeshData_TessellatedShape, IMeshData_StatusOwner
    {

    }
    public abstract class AbstractMeshData_Wire : AbstractMeshData_TessellatedShape, IMeshData_Wire
    {
        protected AbstractMeshData_Wire(TopoDS_Shape theShape) : base(theShape)
        {
        }
    }
}