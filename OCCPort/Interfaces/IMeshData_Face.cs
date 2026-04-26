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

        //! Adds wire to discrete model of face.
        IWireHandle AddWire(TopoDS_Wire theWire, int theEdgeNb = 0);
        
        IWireHandle GetWire(int aWireIt);
    }

}