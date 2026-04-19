using System.Diagnostics;

namespace OCCPort.Interfaces
{
    //! Explores TopoDS_Shape for parts to be meshed - faces and free edges.
    public interface IMeshTools_ShapeExplorer : IMeshData_Shape
    {
        void Accept(IMeshTools_ShapeVisitor aVisitor);
    }


}