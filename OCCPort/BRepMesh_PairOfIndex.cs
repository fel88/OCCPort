using OpenTK.Graphics.ES11;

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

        //! Remove index from the given position.
        //! @param thePairPos position of index in the pair (1 or 2).
        public void RemoveIndex(int thePairPos)
        {
            if (thePairPos != 1 && thePairPos != 2)
                throw new Standard_OutOfRange("BRepMesh_PairOfIndex::RemoveIndex, requested index is out of range");

            if (thePairPos == 1)
                myIndex[0] = myIndex[1];

            myIndex[1] = -1;
        }

        //! Returns first index of pair.
        public int FirstIndex()
        {
            return myIndex[0];
        }
        //! Returns number of initialized indices.
        public int Extent()
        {
            return (myIndex[0] < 0 ? 0 : (myIndex[1] < 0 ? 1 : 2));
        }
        //! Returns is pair is empty.
        public bool IsEmpty()
        {
            // Check only first index. It is impossible to update
            // second index if the first one is empty.
            return (myIndex[0] < 0);
        }
        //! Returns index corresponding to the given position in the pair.
        //! @param thePairPos position of index in the pair (1 or 2).
        public int Index(int thePairPos)
        {
            if (thePairPos != 1 && thePairPos != 2)
                throw new Standard_OutOfRange("BRepMesh_PairOfIndex::Index, requested index is out of range");

            return myIndex[thePairPos - 1];
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