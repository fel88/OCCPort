using System.Reflection.Metadata;
using TKG3d;
using TKMath;

namespace OCCPort
{
    //! Representation of a 2D polygon in the parametric
    //! space of a surface.
    public class BRep_PolygonOnSurface : BRep_CurveRepresentation
    {
        Poly_Polygon2D myPolygon2D;
        Geom_Surface mySurface;

        public BRep_PolygonOnSurface(Poly_Polygon2D P,
                          Geom_Surface S,
                          TopLoc_Location L) : base(L)
        {
            myPolygon2D = (P);
            mySurface = (S);
        }

        public override void Continuity(GeomAbs_Shape shape)
        {
            throw new NotImplementedException();
        }

        public override BRep_CurveRepresentation Copy()
        {
            BRep_PolygonOnSurface P = new BRep_PolygonOnSurface(myPolygon2D,
                                mySurface,
                                Location());
            return P;
        }

        public override Geom_Surface Surface()
        {
            return mySurface;
        }

        

    }
}
