using System.Reflection.Metadata;

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


        public override Geom_Surface Surface()
        {
            return mySurface;
        }

        

    }
}
