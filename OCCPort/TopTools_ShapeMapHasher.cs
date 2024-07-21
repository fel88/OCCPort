namespace OCCPort
{
    //! Hash tool, used for generating maps of shapes in topology.
    class TopTools_ShapeMapHasher
    {

        //! Computes a hash code for the given shape, in the range [1, theUpperBound]
        //! @param theShape the shape which hash code is to be computed
        //! @param theUpperBound the upper bound of the range a computing hash code must be within
        //! @return a computed hash code, in the range [1, theUpperBound]
        static int HashCode(TopoDS_Shape theShape, int theUpperBound)
        {
            return theShape.HashCode(theUpperBound);
        }

        //! Returns True  when the two  keys are the same. Two
        //! same  keys  must   have  the  same  hashcode,  the
        //! contrary is not necessary.
        public static bool IsEqual(TopoDS_Shape S1, TopoDS_Shape S2)
        {
            return S1.IsSame(S2);
        }
    }
}