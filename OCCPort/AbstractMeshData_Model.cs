using OCCPort.Interfaces;

namespace OCCPort
{
    public abstract class AbstractMeshData_Model : AbstractMeshData_Shape, IMeshData_Model
    {
        public AbstractMeshData_Model(TopoDS_Shape theShape) : base(theShape)
        {
        }
        //! Returns number of edges in discrete model.
        public abstract int EdgesNb();

        //! Adds new edge to shape model.
        public abstract IMeshData_Edge AddEdge(TopoDS_Edge theEdge);

        //! Gets model's face with the given index.
        public abstract IMeshData_Face GetFace(int theIndex);


        //! Adds new face to shape model.
        public abstract IMeshData_Face AddFace(TopoDS_Face theFace);

        public abstract IMeshData_Edge GetEdge(int v);
        
    }
}