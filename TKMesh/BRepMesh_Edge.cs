namespace TKMesh
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
        //! Returns movability flag of the Link.
        public BRepMesh_DegreeOfFreedom Movability()
        {
            return myMovability;
        }
        //! Sets movability flag of the Link.
        //! @param theMovability flag to be set.
        public void SetMovability(BRepMesh_DegreeOfFreedom theMovability)
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
        //! Checks for equality with another edge.
        //! @param theOther edge to be checked against this one.
        //! @return TRUE if equal, FALSE if not.
        public bool IsEqual(BRepMesh_Edge theOther)
        {
            if (myMovability == BRepMesh_DegreeOfFreedom.BRepMesh_Deleted || theOther.myMovability == BRepMesh_DegreeOfFreedom.BRepMesh_Deleted)
                return false;

            return IsSameOrientation(theOther) ||
              (FirstNode() == theOther.LastNode() && LastNode() == theOther.FirstNode());
        }

        public override bool Equals(object obj)
        {
            return IsEqual(obj as BRepMesh_Edge);
        }

        BRepMesh_DegreeOfFreedom myMovability;

    }
}

