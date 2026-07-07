using OCCPort.Common;

namespace TKMath
{
    //! Defines an array of 3D nodes of single/double precision configurable at construction time.
    public class Poly_ArrayOfNodes : NCollection_AliasedArray
    {
        //! Empty constructor of double-precision array.
       // Poly_ArrayOfNodes() : NCollection_AliasedArray((Standard_Integer )sizeof(gp_Pnt))
        public Poly_ArrayOfNodes() : base(sizeof(double)*3)
        {

        }
        public gp_Pnt Value(int theIndex)
        {
            if (myStride == (int)sizeof(double) * 3)
            {
                return pnts[theIndex];
                //return base.Value<gp_Pnt>(theIndex);
            }
            else
            {
                var aVec3=pnts3[theIndex];
                //gp_Vec3f aVec3 = base.Value<gp_Vec3f>(theIndex);
                return new gp_Pnt(aVec3.x(), aVec3.y(), aVec3.z());
            }
        }
        public int Length()
        {
            if (pnts == null)//not origin code
                return 0;

            return pnts.Length;
        }

        public void SetValue(int theIndex, gp_Pnt theValue)
        {
            if (myStride == sizeof(double)*3)
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
            return Size() == 0;
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


        //public T Value<T>(int theIndex)
        //{
            
        //    dynamic t = null;
        //    if (typeof(T)==typeof(gp_Pnt) || myStride == (int)sizeof(double) * 3)
        //    {
        //        t = pnts[theIndex];

        //        //return NCollection_AliasedArray::Value<gp_Pnt>(theIndex);
        //    }
        //    else if(typeof(T) == typeof(gp_Vec3f))
        //    {
        //        t = pnts3[theIndex];
        //        //	gp_Vec3 aVec3 = vecs[theIndex]; ;
        //        //const gp_Vec3f&aVec3 = NCollection_AliasedArray::Value<gp_Vec3f>(theIndex);
        //        //return new gp_Pnt(aVec3.x(), aVec3.y(), aVec3.z());
        //    }
        //    return (T)t;
        //}

        internal int Size()
        {
            if (myStride == (int)sizeof(double) * 3)
            {
                return pnts.Length;
            }

            return pnts3.Length;
        }

        internal void Resize(int theNbNodes, bool theToCopyOld)
        {
            var old = pnts;
            var old2 = pnts3;
            pnts = new gp_Pnt[theNbNodes];
            pnts3 = new gp_Vec3f[theNbNodes];
            for (int i = 0; i < pnts.Length; i++)
            {
                pnts[i] = new gp_Pnt();
            }
            for (int i = 0; i < pnts3.Length; i++)
            {
                pnts3[i] = new TKernel.NCollection_Vec3<float> ();
            }
            if (theToCopyOld)
            {
                for (int i = 0; i < Math.Min(old.Length, pnts.Length); i++)
                {
                    pnts[i] = old[i];
                }
                for (int i = 0; i < Math.Min(old.Length, pnts3.Length); i++)
                {
                    pnts3[i] = old2[i];
                }
            }
        }
    }
}