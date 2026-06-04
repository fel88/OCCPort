using OCCPort.Common;
using TKMath;

namespace TKG3d
{
    //! The "XAxis" and the "YAxis" define the placement plane of the
    //! surface (Z = 0, and parametric value V = 0)  perpendicular to
    //! the symmetry axis. The "XAxis" defines the origin of the
    //! parameter U = 0.  The trigonometric sense gives the positive
    //! orientation for the parameter U.
    //!
    //! When you create a CylindricalSurface the U and V directions of
    //! parametrization are such that at each point of the surface the
    //! normal is oriented towards the "outside region".
    //!
    //! The methods UReverse VReverse change the orientation of the
    //! surface.
    public class Geom_CylindricalSurface : Geom_ElementarySurface
    {
        public override void Bounds(out double U1, out double U2, out double V1, out double V2)
        {
            U1 = 0.0; U2 = 2.0 * Math.PI;
            V1 = -Precision.Infinite(); V2 = Precision.Infinite();

        }
        double radius;

        public override Geom_Geometry Copy()
        {
            Geom_CylindricalSurface Cs = new Geom_CylindricalSurface(pos, radius);
            return Cs;
        }
        public double Radius() { return radius; }
        public Geom_CylindricalSurface(gp_Ax3 A3,
                           double R)

        {
            radius = (R);
            if (R < 0.0)
                throw new Standard_ConstructionError();
            pos = A3;
        }


        public override void Transform(gp_Trsf T)
        {
            radius = radius * Math.Abs(T.ScaleFactor());
            pos.Transform(T);
        }

        public override bool IsUPeriodic()
        {
            throw new NotImplementedException();
        }

        public override bool IsVPeriodic()
        {
            throw new NotImplementedException();
        }

        public override void D1(double U, double V, out gp_Pnt P, out gp_Vec D1U, out gp_Vec D1V)
        {
            throw new NotImplementedException();
        }

        public override void D0(double U, double V, ref gp_Pnt P)
        {
            throw new NotImplementedException();
        }

        public override void D2(double U, double V, out gp_Pnt P, out gp_Vec D1U, out gp_Vec D1V, out gp_Vec D2U, out gp_Vec D2V, out gp_Vec D2UV)
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

        public override bool IsUClosed()
        {
            throw new NotImplementedException();
        }

        public override bool IsVClosed()
        {
            throw new NotImplementedException();
        }
    }
}
