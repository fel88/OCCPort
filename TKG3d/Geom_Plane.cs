using TKMath;

namespace TKG3d
{
    //! Describes a plane in 3D space.
    //! A plane is positioned in space by a coordinate system
    //! (a gp_Ax3 object) such that the plane is defined by
    //! the origin, "X Direction" and "Y Direction" of this
    //! coordinate system.
    //! This coordinate system is the "local coordinate
    //! system" of the plane. The following apply:
    //! - Its "X Direction" and "Y Direction" are respectively
    //! the u and v parametric directions of the plane.
    //! - Its origin is the origin of the u and v parameters
    //! (also called the "origin" of the plane).
    //! - Its "main Direction" is a vector normal to the plane.
    //! This normal vector gives the orientation of the
    //! plane only if the local coordinate system is "direct".
    //! (The orientation of the plane is always defined by
    //! the "X Direction" and the "Y Direction" of its local
    //! coordinate system.)
    //! The parametric equation of the plane is:
    //! @code
    //!   P(u, v) = O + u*XDir + v*YDir
    //! @endcode
    //! where O, XDir and YDir are respectively the
    //! origin, the "X Direction" and the "Y Direction" of the
    //! local coordinate system of the plane.
    //! The parametric range of the two parameters u and v
    //! is ] -infinity, +infinity [.
    public class Geom_Plane : Geom_ElementarySurface
    {
        public void Coefficients(ref double A, ref double B, ref double C, ref double D)
        {

            gp_Pln Pl = new gp_Pln(Position());
            Pl.Coefficients(ref A, ref B, ref C, ref D);
        }

        public gp_Pln Pln()
        {

            return new gp_Pln(Position());
        }
        //! P is the "Location" point or origin of the plane.
        //! V is the direction normal to the plane.
        public Geom_Plane(gp_Pnt P, gp_Dir V)
        {
            gp_Pln Pl = new gp_Pln(P, V);
            pos = Pl.Position();
        }

        public Geom_Plane(gp_Pln Pl)
        {
            pos = Pl.Position();

        }
        //! Applies the transformation T to this plane.
        public override void Transform(gp_Trsf T)
        {
            pos.Transform(T);
        }

        //! Creates a new object which is a copy of this plane.
        public override Geom_Geometry Copy()
        {
            Geom_Plane Pl = new Geom_Plane(pos);
            return Pl;
        }
        public Geom_Plane(gp_Ax3 A3)
        {

            pos = A3;
        }

        public override void Bounds(out double U1, out double U2, out double V1, out double V2)
        {
            U1 = -Precision.Infinite();
            U2 = Precision.Infinite();
            V1 = -Precision.Infinite();
            V2 = Precision.Infinite();
        }

        public override bool IsUPeriodic()
        {
            return false;
        }

        public override bool IsVPeriodic()
        {
            return false;
        }

        public override void D1(double U, double V, out gp_Pnt P, out gp_Vec D1U, out gp_Vec D1V)
        {
            P = new gp_Pnt();
            D1U = new gp_Vec();
            D1V = new gp_Vec();
            ElSLib.PlaneD1(U, V, pos, ref P, ref D1U, ref D1V);

        }

        public override void D0(double U, double V, ref gp_Pnt P)
        {
            P = ElSLib.PlaneValue(U, V, pos);

        }

        public override void D2(double U, double V, out gp_Pnt P, out gp_Vec D1U, out gp_Vec D1V, out gp_Vec D2U, out gp_Vec D2V, out gp_Vec D2UV)
        {
            P = new gp_Pnt();
            D1U = new gp_Vec();
            D1V = new gp_Vec();
            D2U = new gp_Vec();
            D2V = new gp_Vec();
            D2UV = new gp_Vec();
            ElSLib.PlaneD1(U, V, pos, ref P, ref D1U, ref D1V);
            D2U.SetCoord(0.0, 0.0, 0.0);
            D2V.SetCoord(0.0, 0.0, 0.0);
            D2UV.SetCoord(0.0, 0.0, 0.0);
        }

        public override Geom_Curve UIso(double U)
        {
            Geom_Line GL = new Geom_Line(ElSLib.PlaneUIso(pos, U));
            return GL;
        }

        public override Geom_Curve VIso(double V)
        {
            Geom_Line GL = new Geom_Line(ElSLib.PlaneVIso(pos, V));
            return GL;
        }

        public override bool IsUClosed()
        {
            return false;
        }

        public override bool IsVClosed()
        {
            return false;
        }
    }
}
