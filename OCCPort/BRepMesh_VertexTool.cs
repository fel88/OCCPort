namespace OCCPort
{
    //! Describes data structure intended to keep mesh nodes 
    //! defined in UV space and implements functionality 
    //! providing their uniqueness regarding their position.
    public class BRepMesh_VertexTool
    {
        BRepMesh_VertexInspector mySelector;
        //! Returns a number of vertices.
        public int Extent()
        {
            return mySelector.NbVertices();
        }
    }
}