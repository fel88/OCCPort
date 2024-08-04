namespace OCCPort
{
    //! Class intended for fast searching of the coincidence points.
    internal class BRepMesh_VertexInspector
    {
        VectorOfVertex myVertices;
        //! Returns number of registered vertices.
        public int NbVertices()
        {
            return myVertices.Length();
        }
    }
}