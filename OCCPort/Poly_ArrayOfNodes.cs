using System.Collections.Generic;

namespace OCCPort
{
    //! Defines an array of 3D nodes of single/double precision configurable at construction time.
    public class Poly_ArrayOfNodes //: public NCollection_AliasedArray<>
    {

        // =======================================================================
        // function : Value
        // purpose  :
        // =======================================================================
        List<gp_Pnt> pnts = new List<gp_Pnt>();
        List<gp_Vec3> vecs = new List<gp_Vec3>();
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
    }
}
