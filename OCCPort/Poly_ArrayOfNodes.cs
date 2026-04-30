using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace OCCPort
{
    //! Defines an array of 3D nodes of single/double precision configurable at construction time.
    public class Poly_ArrayOfNodes //: public NCollection_AliasedArray<>
    {

        public int Length()
        {
            if (pnts == null)//not origin code
                return 0;

            return pnts.Length;
        }

        public void SetValue(int theIndex, gp_Pnt theValue)
        {
            if (myStride == 2)
            {
                //NCollection_AliasedArray::ChangeValue<gp_Pnt>(theIndex) = theValue;
                pnts[theIndex] = theValue;
            }
            else
            {
                //gp_Vec3f & aVec3 = NCollection_AliasedArray::ChangeValue<gp_Vec3f>(theIndex);
                pnts3[theIndex].SetValues((float)theValue.X(), (float)theValue.Y(), (float)theValue.Z());
            }
        }
        public bool IsEmpty()
        {
            return pnts.Length == 0;
        }
        // =======================================================================
        // function : Value
        // purpose  :
        // =======================================================================
        gp_Pnt[] pnts;
        gp_Vec3f[] pnts3;
        //! Returns TRUE if array defines nodes with double precision.
        public bool IsDoublePrecision() { return myStride == 2; }

        //! Sets if array should define nodes with double or single precision.
        //! Raises exception if array was already allocated.
        public void SetDoublePrecision(bool theIsDouble)
        {
            //if (myData != null) { throw Standard_ProgramError("Poly_ArrayOfNodes::SetDoublePrecision() should be called before allocation"); }
            myStride = theIsDouble ? 2 : 3;
        }

        int myStride;    //!< element size
        public gp_Pnt Value(int theIndex)
        {
            //if (myStride == (int)sizeof(typeof(gp_Pnt)))
            {
                return pnts[theIndex];
                //return NCollection_AliasedArray::Value<gp_Pnt>(theIndex);
            }
            //else
            {
                //	gp_Vec3 aVec3 = vecs[theIndex]; ;
                //const gp_Vec3f&aVec3 = NCollection_AliasedArray::Value<gp_Vec3f>(theIndex);
                //return new gp_Pnt(aVec3.x(), aVec3.y(), aVec3.z());
            }
        }

        internal int Size()
        {
            return pnts.Length;
        }

        internal void Resize(int theNbNodes, bool theToCopyOld)
        {
            var old = pnts;
            pnts = new gp_Pnt[theNbNodes];
            if (theToCopyOld)
            {
                for (int i = 0; i < Math.Min(old.Length, pnts.Length); i++)
                {
                    pnts[i] = old[i];
                }
            }
        }
    }
}
