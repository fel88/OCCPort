using OCCPort.Enums;
using System;

namespace OCCPort
{

    //! Light weighted structure representing link of the mesh.
    public class BRepMesh_Edge : BRepMesh_OrientedEdge
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

        //! Checks if the given edge and this one have the same orientation.
        //! @param theOther edge to be checked against this one.
        //! \return TRUE if edges have the same orientation, FALSE if not.
        public bool IsSameOrientation(BRepMesh_Edge theOther)
        {
            return base.IsEqual(theOther);
        }

       

        BRepMesh_DegreeOfFreedom myMovability;
    }
}