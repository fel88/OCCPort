using TKernel;

namespace TKMath
{
    //! Defines an array of 2D nodes of single/double precision configurable at construction time.

    internal class Poly_ArrayOfUVNodes : NCollection_AliasedArray
    {
        //! Resizes the array to specified bounds.
        //! No re-allocation will be done if length of array does not change,
        //! but existing values will not be discarded if theToCopyData set to FALSE.
        //! @param theLength new length of array
        //! @param theToCopyData flag to copy existing data into new array
        internal void Resize(int v1, bool v2)
        {
            var old = points;
            points = new gp_Pnt2d[v1];

            if (v2)
            {
                for (int i = 0; i < Math.Min(old.Length, v1); i++)
                {
                    points[i] = old[i];
                }
            }
        }

        public gp_Pnt2d Value(int index)
        {
            return points[index];
        }
        public bool IsEmpty()
        {
            return points == null || points.Length == 0;
        }

        gp_Pnt2d[] points = null;
        internal int Size()
        {
            return points.Length;
        }

        internal void SetValue(int v, gp_Pnt2d thePnt)
        {
            // if (myStride == (Standard_Integer)sizeof(gp_Pnt2d))
            {
                //  NCollection_AliasedArray::ChangeValue<gp_Pnt2d>(theIndex) = theValue;
            }
            //else
            {
                //gp_Vec2f & aVec2 = NCollection_AliasedArray::ChangeValue<gp_Vec2f>(theIndex);
                //aVec2.SetValues((float)theValue.X(), (float)theValue.Y());
            }
        }
    }
}
