using System.Reflection.Metadata;
using TKMath;

namespace OCCPort
{
    //! Representation by a 3D polygon.
    public class BRep_Polygon3D : BRep_CurveRepresentation
    {
        public BRep_Polygon3D(Poly_Polygon3D P, TopLoc_Location L) : base(L)
        {

            myPolygon3D = (P);
        }
        
        public override Poly_Polygon3D Polygon3D()
        {
            return myPolygon3D;
        }

        public override BRep_CurveRepresentation Copy()
        {
            BRep_Polygon3D P = new BRep_Polygon3D(myPolygon3D, Location());
            return P;
        }

        public override void Polygon3D(Poly_Polygon3D P)
        {
            myPolygon3D = P;

        }

        Poly_Polygon3D myPolygon3D;

    }

}