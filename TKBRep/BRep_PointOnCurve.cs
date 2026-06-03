using System;
using System.Reflection.Metadata;
using TKG3d;
using TKMath;

namespace OCCPort
{
    //! Representation by a parameter on a 3D curve.
    public class BRep_PointOnCurve : BRep_PointRepresentation
    {

        public BRep_PointOnCurve(double P,
                      Geom_Curve C,
                      TopLoc_Location L) : base(P, L)
        {
            myCurve = (C);
        }

        Geom_Curve myCurve;


        public override double Parameter2()
        {
            throw new NotImplementedException();
        }
    }
}