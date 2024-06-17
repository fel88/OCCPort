using System;

namespace OCCPort
{
    //! Representation of a curve by a 3D curve.

    internal class BRep_Curve3D : BRep_GCurve
    {
        public BRep_Curve3D(Geom_Curve C, TopLoc_Location L)
            : base(L, C == null ? RealFirst() : C.FirstParameter(),
C == null ? RealLast() : C.LastParameter())
        {

            myCurve = C;

        }

        Geom_Curve myCurve;

        private static double RealLast()
        {
            throw new NotImplementedException();
        }

        private static double RealFirst()
        {
            throw new NotImplementedException();
        }

        public void SetRange(double First, double Last)
        {
            myFirst = First;
            myLast = Last;
            Update();
        }

    }
}
