using System.Reflection.Metadata;

namespace OCCPort
{
    //! Representation by two 2d polygons in the parametric
    //! space of a surface.
    public class BRep_PolygonOnClosedSurface : BRep_PolygonOnSurface
    {
        public BRep_PolygonOnClosedSurface(Poly_Polygon2D P1,
                               Poly_Polygon2D P2,
                               Geom_Surface S,
                                TopLoc_Location L) :
                             base(P1, S, L)
        {
            myPolygon2 = (P2);

        }
        Poly_Polygon2D myPolygon2;

    }
}
