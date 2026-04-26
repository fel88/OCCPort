using System.Reflection.Metadata;

namespace OCCPort
{
    //! Representation by a 3D polygon.
    public class BRep_Polygon3D : BRep_CurveRepresentation
    {
        public BRep_Polygon3D(Poly_Polygon3D P, TopLoc_Location L) : base(L)
        {

            myPolygon3D = (P);
        }
        
        public Poly_Polygon3D Polygon3D()
        {
            return myPolygon3D;
        }
        Poly_Polygon3D myPolygon3D;

    }

}