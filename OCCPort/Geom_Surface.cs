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
        public abstract void Bounds(ref double U1, ref double U2, 
            ref double V1, ref double V2);

    }

}