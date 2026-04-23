namespace OCCPort.Interfaces
{
    public abstract class AbstractMeshData_Wire : AbstractMeshData_TessellatedShape, IMeshData_Wire
    {
        protected AbstractMeshData_Wire(TopoDS_Shape theShape) : base(theShape)
        {
        }
       

        public abstract int EdgesNb();

        public abstract IMeshData_Edge GetEdge(int aEdgeIt);
        public abstract TopAbs_Orientation GetEdgeOrientation(int aEdgeIt);
    }
}