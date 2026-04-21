namespace OCCPort.Interfaces
{
    //! Interface class representing discrete model of an edge.
    public interface IMeshData_Edge : IMeshData_TessellatedShape, IMeshData_StatusOwner
    {
        //! Returns 3d curve associated with current edge.
        ICurveHandle GetCurve();
        IPCurveHandle GetPCurve(IMeshData_Face myDFace, TopAbs_Orientation topAbs_Orientation);

    }
}