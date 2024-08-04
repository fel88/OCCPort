namespace OCCPort
{
    //! Interface class representing discrete model of an edge.
    public abstract class IMeshData_Edge : IMeshData_TessellatedShape, IMeshData_StatusOwner
    {
        protected IMeshData_Edge(TopoDS_Shape theShape) : base(theShape)
        {
        }
    }
}