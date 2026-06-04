using System.Reflection.Metadata;
using TKG2d;
using TKG3d;
using TKMath;

namespace OCCPort
{
    //! Representation by   a parameter on  a curve   on a
    //! surface.
    public class BRep_PointOnCurveOnSurface : BRep_PointsOnSurface
    {
        public BRep_PointOnCurveOnSurface
  (double P,
   Geom2d_Curve C,
   Geom_Surface S,
   TopLoc_Location L) : base(P, S, L)
        {
            myPCurve = (C);

        }

        Geom2d_Curve myPCurve;

    }
}

