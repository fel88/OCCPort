namespace OCCPort
{
    //! Interface class representing discrete model of a wire.
    //! Wire should represent an ordered set of edges.
    public abstract class IMeshData_Wire : IMeshData_TessellatedShape, IMeshData_StatusOwner
    {
        protected IMeshData_Wire(TopoDS_Shape theShape) : base(theShape)
        {
        }
    }
}