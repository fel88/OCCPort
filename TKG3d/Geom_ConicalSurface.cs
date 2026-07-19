using TKMath;

namespace TKG3d
{
    //! Describes a cone.
    //! A cone is defined by the half-angle (can be negative) at its apex, and
    //! is positioned in space by a coordinate system (a
    //! gp_Ax3 object) and a reference radius as follows:
    //! - The "main Axis" of the coordinate system is the
    //! axis of revolution of the cone.
    //! - The plane defined by the origin, the "X Direction"
    //! and the "Y Direction" of the coordinate system is
    //! the reference plane of the cone. The intersection
    //! of the cone with this reference plane is a circle of
    //! radius equal to the reference radius.
    //! - The apex of the cone is on the negative side of
    //! the "main Axis" of the coordinate system if the
    //! half-angle is positive, and on the positive side if
    //! the half-angle is negative.
    //! This coordinate system is the "local coordinate
    //! system" of the cone. The following apply:
    //! - Rotation around its "main Axis", in the
    //! trigonometric sense given by the "X Direction"
    //! and the "Y Direction", defines the u parametric direction.
    //! - Its "X Axis" gives the origin for the u parameter.
    //! - Its "main Direction" is the v parametric direction of the cone.
    //! - Its origin is the origin of the v parameter.
    //! The parametric range of the two parameters is:
    //! @code
    //!  - [ 0, 2.*Pi ] for u, and
    //!  - ] -infinity, +infinity [ for v
    //! @endcode
    //! The parametric equation of the cone is:
    //! @code
    //! P(u, v) = O + (R + v*sin(Ang)) * (cos(u)*XDir + sin(u)*YDir) + v*cos(Ang)*ZDir
    //! @endcode
    //! where:
    //! - O, XDir, YDir and ZDir are respectively
    //! the origin, the "X Direction", the "Y Direction" and
    //! the "Z Direction" of the cone's local coordinate system,
    //! - Ang is the half-angle at the apex of the cone,   and
    //! - R is the reference radius.
    public class Geom_ConicalSurface : Geom_ElementarySurface
    {
        public override void Bounds(out double U1, out double U2, out double V1, out double V2)
        {
            throw new NotImplementedException();
        }

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
