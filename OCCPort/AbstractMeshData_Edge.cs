using OCCPort.Interfaces;

namespace OCCPort
{
    public abstract class AbstractMeshData_Edge : AbstractMeshData_TessellatedShape, IMeshData_StatusOwner
    {
        protected AbstractMeshData_Edge(TopoDS_Shape theShape) : base(theShape)
        {
        }
    }
}