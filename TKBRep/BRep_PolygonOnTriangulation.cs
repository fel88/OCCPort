using OCCPort.Tester;
using System.Reflection.Metadata;
using TKMath;

namespace OCCPort
{
    //! A representation by an array of nodes on a
    //! triangulation.
    public class BRep_PolygonOnTriangulation : BRep_CurveRepresentation
    {
        public BRep_PolygonOnTriangulation(Poly_PolygonOnTriangulation P,
 Poly_Triangulation T,
  TopLoc_Location L) : base(L)
        {

            myPolygon = (P);
            myTriangulation = (T);
        }
        Poly_PolygonOnTriangulation myPolygon;
        Poly_Triangulation myTriangulation;

        public override Poly_PolygonOnTriangulation PolygonOnTriangulation()
        {
            return myPolygon;
        }
        public override bool IsPolygonOnTriangulation()
        {
            return true;
        }
        public override bool IsPolygonOnTriangulation(Poly_Triangulation T, TopLoc_Location L)
        {
            return (T == myTriangulation) && (L == myLocation);

        }

        public Poly_Triangulation Triangulation()
        {
            return myTriangulation;
        }


        public override BRep_CurveRepresentation Copy()
        {
            BRep_PolygonOnTriangulation P =
   new BRep_PolygonOnTriangulation(myPolygon, myTriangulation, Location());

            return P;
        }

        public override void Continuity(GeomAbs_Shape shape)
        {
            throw new NotImplementedException();
        }
    }
}
