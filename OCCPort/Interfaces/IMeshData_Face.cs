namespace OCCPort.Interfaces
{
    //! Interface class representing discrete model of a face.
    //! Face model contains one or several wires.
    //! First wire is always outer one.
    public interface IMeshData_Face : IMeshData_TessellatedShape, IMeshData_StatusOwner
    {
        //! Returns face's surface.
        BRepAdaptor_Surface GetSurface();
        TopoDS_Face GetFace();
        int WiresNb();
        void SetStatus(IMeshData_Status meshData_Failure);



    }

}