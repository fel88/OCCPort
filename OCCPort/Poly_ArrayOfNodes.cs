using System;
using System.Collections.Generic;

namespace OCCPort
{
    //! Defines an array of 3D nodes of single/double precision configurable at construction time.
    public class Poly_ArrayOfNodes //: public NCollection_AliasedArray<>
    {

        public int Length()
        {
            return pnts.Length;
        }

        // =======================================================================
        // function : Value
        // purpose  :
        // =======================================================================
        gp_Pnt[] pnts;

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
