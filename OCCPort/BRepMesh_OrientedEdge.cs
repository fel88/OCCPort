namespace OCCPort
{
    internal class BRepMesh_OrientedEdge
    {
        public BRepMesh_OrientedEdge()
        {
            myFirstNode = (-1);
            myLastNode = -1;
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

            throw new System.NotImplementedException();
        }

        public override int GetHashCode()
        {
            return (myFirstNode + myLastNode);

        }
    }
}