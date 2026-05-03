namespace OCCPort
{
    //! Sort two points in projection on vector (1,1)
    class ComparatorOfIndexedVertexOfDelaun
    {
        public ComparatorOfIndexedVertexOfDelaun(BRepMesh_DataStructureOfDelaun theDS)
        {

            myStructure = (theDS);
        }

        BRepMesh_DataStructureOfDelaun myStructure;

    }
}