using System.Reflection.Metadata;

namespace OCCPort
{
    //! triangulation.
    public class BRep_PolygonOnClosedTriangulation : BRep_PolygonOnTriangulation
    {


        public BRep_PolygonOnClosedTriangulation(Poly_PolygonOnTriangulation P1, Poly_PolygonOnTriangulation P2, Poly_Triangulation t, TopLoc_Location l)
            : base(P1, t, l)
        {
            myPolygon2 = (P2);
        }

        public override bool IsPolygonOnClosedTriangulation()
        {

            return true;
        }

        public override Poly_PolygonOnTriangulation PolygonOnTriangulation2()
        {


            return myPolygon2;

        }
        Poly_PolygonOnTriangulation myPolygon2;

    }
}

