using OCCPort.Interfaces;

namespace OCCPort
{
    //! Default implementation of face data model entity.
    public class BRepMeshData_Face : AbstractMeshData_Face, IMeshData_Face
    {
        public BRepMeshData_Face(TopoDS_Face theFace) : base(theFace)
        {
            //myDWires(256, myAllocator)
        }

    }

}