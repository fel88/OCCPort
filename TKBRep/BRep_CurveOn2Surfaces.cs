using System.Reflection.Metadata;
using TKG3d;
using TKMath;

namespace OCCPort
{
    //! Defines a continuity between two surfaces.
    public class BRep_CurveOn2Surfaces : BRep_CurveRepresentation
    {
        public BRep_CurveOn2Surfaces(TopLoc_Location L) : base(L)
        {
        }
        public BRep_CurveOn2Surfaces(Geom_Surface S1,
                            Geom_Surface S2,
                            TopLoc_Location L1,
                            TopLoc_Location L2,
                            GeomAbs_Shape C) : base(L1)
        {
            mySurface = (S1);
            mySurface2 = (S2);
            myLocation2 = (L2);
            myContinuity = (C);

        }

        Geom_Surface mySurface;
        Geom_Surface mySurface2;
        TopLoc_Location myLocation2;
        GeomAbs_Shape myContinuity;

        public override BRep_CurveRepresentation Copy()
        {
            throw new System.NotImplementedException();
        }

        public override void Continuity(GeomAbs_Shape shape)
        {
            throw new NotImplementedException();
        }
    }
}