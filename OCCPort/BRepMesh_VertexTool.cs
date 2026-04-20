namespace OCCPort
{
    //! Describes data structure intended to keep mesh nodes 
    //! defined in UV space and implements functionality 
    //! providing their uniqueness regarding their position.
    public class BRepMesh_VertexTool
    {
        BRepMesh_VertexInspector mySelector;

        public BRepMesh_VertexTool(NCollection_IncAllocator myAllocator)
        {
            mySelector = new BRepMesh_VertexInspector(myAllocator);
        }

        //! Returns a number of vertices.
        public int Extent()
        {
            return mySelector.NbVertices();
        }
    }
}