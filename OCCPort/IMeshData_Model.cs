namespace OCCPort
{
    public abstract class IMeshData_Model : IMeshData_Shape
    {
        public IMeshData_Model(TopoDS_Shape theShape) : base(theShape)
        {
        }
        //! Returns number of edges in discrete model.
        public abstract int EdgesNb();

        //! Adds new edge to shape model.
        public abstract IMeshData_Edge AddEdge(TopoDS_Edge theEdge);

        //! Adds new face to shape model.
        public abstract IMeshData_Face AddFace(TopoDS_Face theFace);

        //! Returns number of faces in discrete model.
        public abstract int FacesNb();
    }
}