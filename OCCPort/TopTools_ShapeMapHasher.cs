using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OCCPort
{
    //! Hash tool, used for generating maps of shapes in topology.
    public class TopTools_ShapeMapHasher : IEqualityComparer<TopoDS_Shape>
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

        public bool Equals(TopoDS_Shape x, TopoDS_Shape y)
        {
            return IsEqual(x, y);
        }

        public int GetHashCode([DisallowNull] TopoDS_Shape obj)
        {
            return HashCode(obj, int.MaxValue);
            //return obj.GetHashCode();
        }
    }

}