using System.Collections.Generic;

namespace OCCPort
{
    //! Sort two points in projection on vector (1,1)
    class ComparatorOfIndexedVertexOfDelaun : IComparer<int>
    {
        public ComparatorOfIndexedVertexOfDelaun(BRepMesh_DataStructureOfDelaun theDS)
        {

            myStructure = (theDS);
        }

        BRepMesh_DataStructureOfDelaun myStructure;

        public int Compare(int theLeft, int theRight)
        {
            BRepMesh_Vertex aLeft = myStructure.GetNode(theLeft);
            BRepMesh_Vertex aRight = myStructure.GetNode(theRight);
            return new ComparatorOfVertexOfDelaun().Compare(aLeft, aRight);
        }
    }
}