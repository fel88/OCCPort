using System.Numerics;
using TKernel;

namespace TKService
{
    internal class TransformMatrices<Elem_t> where Elem_t : struct, INumber<Elem_t>, IMultiplyOperators<Elem_t, Elem_t, Elem_t>
    {
        public TransformMatrices()
        {
            myIsOrientationValid = false;
            myIsProjectionValid = false;

        }


        bool myIsOrientationValid;
        bool myIsProjectionValid;

        //! Initialize orientation.
        public void InitOrientation()
        {
            myIsOrientationValid = true;
            Orientation = new NCollection_Mat4<Elem_t>();
            Orientation.InitIdentity();
        }



        //! Invalidate orientation.
        public void ResetOrientation() { myIsOrientationValid = false; }

        //! Invalidate projection.
        public void ResetProjection() { myIsProjectionValid = false; }



        //! Return true if Orientation was not invalidated.
        public bool IsOrientationValid() { return myIsOrientationValid; }

        //! Return true if Projection was not invalidated.
        public bool IsProjectionValid() { return myIsProjectionValid; }
        public NCollection_Mat4<Elem_t> Orientation;

        public NCollection_Mat4<Elem_t> MProjection;
        public NCollection_Mat4<Elem_t> LProjection;
        public NCollection_Mat4<Elem_t> RProjection;

        //! Initialize projection.
        public void InitProjection()
        {
            myIsProjectionValid = true;
            MProjection = new NCollection_Mat4<Elem_t>();
            LProjection = new NCollection_Mat4<Elem_t>();
            RProjection = new NCollection_Mat4<Elem_t>();
            MProjection.InitIdentity();
            LProjection.InitIdentity();
            RProjection.InitIdentity();
        }
    }
}
