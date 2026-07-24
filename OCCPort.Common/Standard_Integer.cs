
namespace OCCPort.Common
{
    public static class Standard_Integer
    {
        // ------------------------------------------------------------------
        // IntegerLast : Returns the maximum value of an integer
        // ------------------------------------------------------------------
        public static int IntegerLast()
        { return int.MaxValue; }
        // ------------------------------------------------------------------
        // IntegerFirst : Returns the minimum value of an integer
        // ------------------------------------------------------------------
        public static int IntegerFirst()
        { return int.MinValue; }

        public static int HashCode(int theValue, int theUpperBound)
        {
            return IntegerHashCode(theValue, IntegerLast(), theUpperBound);

        }
        //! Computes a hash code for the given value of some integer type, in range [1, theUpperBound]
        //! @tparam TheInteger the type of the integer which hash code is to be computed
        //! @param theValue the value of the TheInteger type which hash code is to be computed
        //! @param theMask the mask for the last bits of the value that are used in the computation of a hash code
        //! @param theUpperBound the upper bound of the range a computing hash code must be within
        //! @return a computed hash code, in range [1, theUpperBound]

        public static int IntegerHashCode(int theValue,
                   int theMask,
                   int theUpperBound)
        {
            return ((theValue & theMask) % theUpperBound + 1);
        }
    }

    public static class Standard_Size
    {
        public static int HashCode(int theValue, int theUpperBound)
        {
            int aKey = ~theValue + (theValue << 18);
            aKey ^= (aKey >> 31);
            aKey *= 21;
            aKey ^= (aKey >> 11);
            aKey += (aKey << 6);
            aKey ^= (aKey >> 22);
            return Standard_Integer. IntegerHashCode(aKey, Standard_Integer.IntegerLast(), theUpperBound);
        }
    }
}
