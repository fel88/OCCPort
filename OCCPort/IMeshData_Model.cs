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
        public abstract IMeshData.IEdgeHandle AddEdge(TopoDS_Edge theEdge);

        //! Returns number of faces in discrete model.
        public abstract int FacesNb();
    }
    //! Default implementation of edge data model entity.
    public class BRepMeshData_Edge : IMeshData_Edge
    {
        public override IMeshData.IEdgeHandle AddEdge(TopoDS_Edge theEdge)
        {
            throw new System.NotImplementedException();
        }
    }
    //! Interface class representing discrete model of an edge.
    public abstract class IMeshData_Edge : IMeshData_TessellatedShape, IMeshData_StatusOwner
    {

    }
    //! Interface class representing shaped model with deflection.
    public abstract class IMeshData_TessellatedShape : IMeshData_Shape
    {

    }
    //! Extension interface class providing status functionality.
    interface IMeshData_StatusOwner
    {

    }
}