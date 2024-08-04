namespace OCCPort
{
    //! Interface class representing discrete model of a face.
    //! Face model contains one or several wires.
    //! First wire is always outer one.
    public abstract class IMeshData_Face : IMeshData_TessellatedShape, IMeshData_StatusOwner
    {
        //! Constructor.
        //! Initializes empty model.
        public IMeshData_Face(TopoDS_Face theFace)
         : base(theFace)
        {
            BRepAdaptor_Surface aSurfAdaptor = new BRepAdaptor_Surface(GetFace(), false);
            //mySurface = new BRepAdaptor_Surface(aSurfAdaptor);
            mySurface = aSurfAdaptor;//todo:Clone??
        }
        //! Returns face's surface.
      public   BRepAdaptor_Surface GetSurface() 
        {
    return mySurface;
  }

    BRepAdaptor_Surface mySurface;
        //! Returns TopoDS_Face attached to model.
        public TopoDS_Face GetFace()
        {
            return TopoDS.Face(GetShape());
        }

    }
}