namespace OCCPort.Interfaces
{
    public interface IMeshData_Model : IMeshData_Shape
    {
        //=======================================================================
        int FacesNb();
        IMeshData_Edge AddEdge(TopoDS_Edge theEdge);
        int EdgesNb();
        IMeshData_Face AddFace(TopoDS_Face theFace);
        IMeshData_Face GetFace(int theFaceIndex);
    }
}