using TKMath;

namespace TKG3d
{
    //! Describes the common behavior for surfaces
    //! constructed by sweeping a curve with another curve.
    //! The Geom package provides two concrete derived
    //! surfaces: surface of revolution (a revolved surface),
    //! and surface of linear extrusion (an extruded surface).
    public class Geom_SweptSurface : Geom_Surface
    {
        public override void Bounds(out double U1, out double U2, out double V1, out double V2)
        {
            throw new NotImplementedException();
        }

        protected Geom_Curve basisCurve;
        protected gp_Dir direction;
        protected GeomAbs_Shape smooth;
        public override Geom_Geometry Copy()
        {
            throw new NotImplementedException();
        }

        public override void D0(double U, double V, ref gp_Pnt P)
        {
            throw new NotImplementedException();
        }

        public override void D1(double U, double V, out gp_Pnt P, out gp_Vec D1U, out gp_Vec D1V)
        {
            throw new NotImplementedException();
        }

        public override void D2(double U, double V, out gp_Pnt P, out gp_Vec D1U, out gp_Vec D1V, out gp_Vec D2U, out gp_Vec D2V, out gp_Vec D2UV)
        {
            throw new NotImplementedException();
        }

        public override bool IsUClosed()
        {
            throw new NotImplementedException();
        }

        public override bool IsUPeriodic()
        {
            throw new NotImplementedException();
        }

        public override bool IsVClosed()
        {
            throw new NotImplementedException();
        }

        public override bool IsVPeriodic()
        {
            throw new NotImplementedException();
        }

        public override void Transform(gp_Trsf t)
        {
            throw new NotImplementedException();
        }

        public override Geom_Curve UIso(double U)
        {
            throw new NotImplementedException();
        }

        public override Geom_Curve VIso(double V)
        {
            throw new NotImplementedException();
        }
    }
}
