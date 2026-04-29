namespace OCCPort
{
    //! This class represents a pair of integer indices to store 
    //! element indices connected to link. It is restricted to 
    //! store more than two indices in it.
    public class BRepMesh_PairOfIndex
    {
        //! Default constructor
        public BRepMesh_PairOfIndex()
        {
            Clear();
        }

        //! Clears indices.
        void Clear()
        {
            myIndex[0] = myIndex[1] = -1;
        }
        int[] myIndex = new int[2];

        //! Appends index to the pair.
        public void Append(int theIndex)
        {
            if (myIndex[0] < 0)
                myIndex[0] = theIndex;
            else
            {
                if (myIndex[1] >= 0)
                    throw new Standard_OutOfRange("BRepMesh_PairOfIndex::Append, more than two index to store");

                myIndex[1] = theIndex;
            }
        }

    }
}