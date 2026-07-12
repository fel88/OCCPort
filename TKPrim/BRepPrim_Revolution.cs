using OCCPort;
using System.Reflection.Metadata;
using TKBRep;
using TKG2d;
using TKG3d;
using TKMath;

namespace TKPrim
{
    //! Implement  the OneAxis algorithm   for a revolution
    //! surface.
    public class BRepPrim_Revolution : BRepPrim_OneAxis

    {
        public BRepPrim_Revolution(gp_Ax2 A, int VMin, double VMax) : base(new BRepPrim_Builder(), A, VMin, VMax)
        {
        }

        public void Meridian(Geom_Curve M,
                     Geom2d_Curve PM)
        {
            myMeridian = M;
            myPMeridian = PM;
        }

        public override gp_Pnt2d MeridianValue(double V)
        {
            throw new NotImplementedException();
        }

        public override TopoDS_Face MakeEmptyLateralFace()
        {
            throw new NotImplementedException();
        }

        public override void SetMeridianPCurve(TopoDS_Edge E, TopoDS_Face F)
        {
            throw new NotImplementedException();
        }

        public override TopoDS_Edge MakeEmptyMeridianEdge(double Ang)
        {
            throw new NotImplementedException();
        }

        Geom_Curve myMeridian;
        Geom2d_Curve myPMeridian;
    }

}
