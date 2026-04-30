using System;

namespace OCCPort
{
    //! Describes the common behavior of surfaces in 3D space.
    //! The Geom package provides many implementations of concrete derived surfaces,
    //! such as planes, cylinders, cones, spheres and tori, surfaces of linear extrusion,
    //! surfaces of revolution, Bezier and BSpline surfaces, and so on.
    //! The key characteristic of these surfaces is that they are parameterized.
    //! Geom_Surface demonstrates:
    //! - how to work with the parametric equation of a surface
    //!   to compute the point of parameters (u, v), and, at this point, the 1st, 2nd ... Nth derivative;
    //! - how to find global information about a surface in
    //!   each parametric direction (for example, level of continuity, whether the surface is closed,
    //!   its periodicity, the bounds of the parameters and so on);
    //! - how the parameters change when geometric transformations are applied to the surface,
    //!   or the orientation is modified.
    //!
    //! Note that all surfaces must have a geometric continuity, and any surface is at least "C0".
    //! Generally, continuity is checked at construction time or when the curve is edited.
    //! Where this is not the case, the documentation makes this explicit.
    //!
    //! Warning
    //! The Geom package does not prevent the construction of
    //! surfaces with null areas, or surfaces which self-intersect.
    public abstract class Geom_Surface : Geom_Geometry
    {
        internal Type DynamicType()
        {
            return GetType();
        }
        //! Returns the parametric bounds U1, U2, V1 and V2 of this surface.
        //! If the surface is infinite, this function can return a value
        //! equal to Precision::Infinite: instead of Standard_Real::LastReal.
        public abstract void Bounds(out double U1, out double U2,
            out double V1, out double V2);

        //! Computes the point P and the first derivatives in the directions U and V at this point.
        //! Raised if the continuity of the surface is not C1.
        //!
        //! Tip: use GeomLib::NormEstim() to calculate surface normal at specified (U, V) point.
        public abstract void D1(double U, double V, out gp_Pnt P, out gp_Vec D1U, out gp_Vec D1V);
        //! Computes the point P, the first and the second derivatives in
        //! the directions U and V at this point.
        //! Raised if the continuity of the surface is not C2.
        public abstract void D2(double U, double V, out gp_Pnt P, out gp_Vec D1U, out gp_Vec D1V, out gp_Vec D2U, out gp_Vec D2V,
            out gp_Vec D2UV);

        public gp_Pnt Value(double U,

                double V)
        {
            gp_Pnt P = new gp_Pnt();
            D0(U, V, ref P);
            return P;
        }

        //! Computes the point of parameter U,V on the surface.
        //!
        //! Raised only for an "OffsetSurface" if it is not possible to
        //! compute the current point.
        public abstract void D0(double U, double V, ref gp_Pnt P);

        public double VPeriod()
        {
            Exceptions.Standard_NoSuchObject_Raise_if
              (!IsVPeriodic(), "Geom_Surface::VPeriod");

            double U1, U2, V1, V2;
            Bounds(out U1, out U2, out V1, out V2);
            return (V2 - V1);
        }

        public double UPeriod()
        {
            Exceptions.Standard_NoSuchObject_Raise_if(!IsUPeriodic(), "Geom_Surface::UPeriod");

            double U1, U2, V1, V2;
            Bounds(out U1, out U2, out V1, out V2);
            return (U2 - U1);
        }

        //! Checks if this surface is periodic in the u parametric direction.
        //! Returns true if:
        //! - this surface is closed in the u parametric direction, and
        //! - there is a constant T such that the distance
        //!   between the points P (u, v) and P (u + T, v)
        //!   (or the points P (u, v) and P (u, v + T)) is less than or equal to gp::Resolution().
        //!
        //! Note: T is the parametric period in the u parametric direction.
        public abstract bool IsUPeriodic();
        //! Checks if this surface is periodic in the v parametric direction.
        //! Returns true if:
        //! - this surface is closed in the v parametric direction, and
        //! - there is a constant T such that the distance
        //!   between the points P (u, v) and P (u + T, v)
        //!   (or the points P (u, v) and P (u, v + T)) is less than or equal to gp::Resolution().
        //!
        //! Note: T is the parametric period in the v parametric direction.
        public abstract bool IsVPeriodic();

    }

}