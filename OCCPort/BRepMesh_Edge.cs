using OCCPort.Enums;

namespace OCCPort
{
    internal class BRepMesh_Edge : BRepMesh_OrientedEdge
    {
        //! Constructs a link between two vertices.
        public BRepMesh_Edge(
        int theFirstNode,
        int theLastNode,
        BRepMesh_DegreeOfFreedom theMovability)
       : base(theFirstNode, theLastNode)

        {
            myMovability = theMovability;
        }
        BRepMesh_DegreeOfFreedom myMovability;
    }
}