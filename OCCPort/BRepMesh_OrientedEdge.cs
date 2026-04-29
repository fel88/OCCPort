using OpenTK.Graphics.ES20;

namespace OCCPort
{
    public class BRepMesh_OrientedEdge
    {
        public BRepMesh_OrientedEdge()
        {
            myFirstNode = (-1);
            myLastNode = -1;
        }

        //! Returns index of first node of the Link.
        public int FirstNode()
        {
            return myFirstNode;
        }

        //! Returns index of last node of the Link.
        public int LastNode()
        {
            return myLastNode;
        }

        //! Constructs a link between two vertices.
        public BRepMesh_OrientedEdge(
             int theFirstNode,
             int theLastNode)
        {
            myFirstNode = (theFirstNode);
            myLastNode = (theLastNode);

        }
        int myFirstNode;
        int myLastNode;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (ReferenceEquals(obj, null))
            {
                return false;
            }
            return IsEqual(obj as BRepMesh_OrientedEdge);
            throw new System.NotImplementedException();
        }

        public override int GetHashCode()
        {
            return (myFirstNode + myLastNode);

        }

        //! Checks this and other edge for equality.
        //! @param theOther edge to be checked against this one.
        //! @return TRUE if edges have the same orientation, FALSE if not.
        protected bool IsEqual(BRepMesh_OrientedEdge theOther)
        {

            return (myFirstNode == theOther.myFirstNode && myLastNode == theOther.myLastNode);

        }
    }
}