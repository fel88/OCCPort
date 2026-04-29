using OCCPort.Tester;
using System.Reflection.Metadata;

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

    }
}
