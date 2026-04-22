using OCCPort.Interfaces;

namespace OCCPort
{
    public abstract class AbstractMeshData_Face : AbstractMeshData_TessellatedShape, IMeshData_Face, IMeshData_StatusOwner
    {
        //! Constructor.
        //! Initializes empty model.
        public AbstractMeshData_Face(TopoDS_Face theFace)
         : base(theFace)
        {
            BRepAdaptor_Surface aSurfAdaptor = new BRepAdaptor_Surface(GetFace(), false);
            //mySurface = new BRepAdaptor_Surface(aSurfAdaptor);
            mySurface = aSurfAdaptor;//todo:Clone??
        }
        //=======================================================================
        public IWireHandle AddWire(
  TopoDS_Wire theWire,
  int theEdgeNb)
        {
            IWireHandle aWire = new BRepMeshData_Wire(theWire, theEdgeNb, myAllocator);
            myDWires.Append(aWire);
            return GetWire(WiresNb() - 1);
        }
        NCollection_IncAllocator myAllocator;

        VectorOfIWireHandles myDWires = new VectorOfIWireHandles();
        public int WiresNb()
        {
            return myDWires.Size();
        }
        //! Returns face's surface.
        public BRepAdaptor_Surface GetSurface()
        {
            return mySurface;
        }

        BRepAdaptor_Surface mySurface;
        //! Returns TopoDS_Face attached to model.
        public TopoDS_Face GetFace()
        {
            return TopoDS.Face(GetShape());
        }

        public void SetStatus(IMeshData_Status meshData_Failure)
        {

        }

        public IWireHandle GetWire(int theIndex)
        {
            return myDWires.Get(theIndex);
        }
    }
}