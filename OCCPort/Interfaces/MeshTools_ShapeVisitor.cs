namespace OCCPort.Interfaces
{
    public abstract class MeshTools_ShapeVisitor : IMeshTools_ShapeVisitor
    {
        public abstract bool addWire(
   TopoDS_Wire theWire,
   IMeshData_Face theDFace);
        public abstract void Visit(TopoDS_Face theFace);
        public abstract void Visit(TopoDS_Edge theEdge);
    }
}