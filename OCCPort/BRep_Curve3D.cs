using System;
using System.Diagnostics.Contracts;
using System.Reflection.Metadata;

namespace OCCPort
{
    //! Representation of a curve by a 3D curve.

    internal class BRep_Curve3D : BRep_GCurve
    {
        public BRep_Curve3D(Geom_Curve C, TopLoc_Location L)
            : base(L, C == null ? Standard_Real.RealFirst() : C.FirstParameter(),
C == null ? Standard_Real.RealLast() : C.LastParameter())
        {
            myCurve = C;
        }

        public override Geom_Curve Curve3D()
        {
            return myCurve;
        }


        Geom_Curve myCurve;



        public override bool IsCurve3D()
        {
            return true;
        }

        public void SetRange(double First, double Last)
        {
            myFirst = First;
            myLast = Last;
            Update();
        }

    }
}
