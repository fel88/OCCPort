namespace OCCPort.Interfaces
{
    //! Interface class representing discrete model of an edge.
    public interface IMeshData_Edge : IMeshData_TessellatedShape, IMeshData_StatusOwner
    {
        //! Returns 3d curve associated with current edge.
        ICurveHandle GetCurve();
        IMeshData_PCurve GetPCurve(IMeshData_Face myDFace, TopAbs_Orientation topAbs_Orientation);

        //! Adds discrete pcurve for the specified discrete face.
        IMeshData_PCurve AddPCurve(
            IMeshData_Face theDFace,
            TopAbs_Orientation theOrientation);


    }
}