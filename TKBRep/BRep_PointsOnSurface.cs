using System;
using System.Reflection.Metadata;
using TKG3d;
using TKMath;

namespace OCCPort
{
    //! Root for points on surface.
    public class BRep_PointsOnSurface : BRep_PointRepresentation
    {
        Geom_Surface mySurface;

        public override double Parameter2()
        {
            throw new NotImplementedException();
        }
        public BRep_PointsOnSurface(double P,

                       Geom_Surface S,
                       TopLoc_Location L) :
       base(P, L)
        {
            mySurface = (S);

        }
    }
}

