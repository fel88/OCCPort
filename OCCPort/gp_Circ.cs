namespace OCCPort
{
    //! Describes a circle in 3D space.
    //! A circle is defined by its radius and positioned in space
    //! with a coordinate system (a gp_Ax2 object) as follows:
    //! -   the origin of the coordinate system is the center of the circle, and
    //! -   the origin, "X Direction" and "Y Direction" of the
    //! coordinate system define the plane of the circle.
    //! This positioning coordinate system is the "local
    //! coordinate system" of the circle. Its "main Direction"
    //! gives the normal vector to the plane of the circle. The
    //! "main Axis" of the coordinate system is referred to as
    //! the "Axis" of the circle.
    //! Note: when a gp_Circ circle is converted into a
    //! Geom_Circle circle, some implicit properties of the
    //! circle are used explicitly:
    //! -   the "main Direction" of the local coordinate system
    //! gives an implicit orientation to the circle (and defines
    //! its trigonometric sense),
    //! -   this orientation corresponds to the direction in
    //! which parameter values increase,
    //! -   the starting point for parameterization is that of the
    //! "X Axis" of the local coordinate system (i.e. the "X Axis" of the circle).
    //! See Also
    //! gce_MakeCirc which provides functions for more complex circle constructions
    //! Geom_Circle which provides additional functions for
    //! constructing circles and works, in particular, with the
    //! parametric equations of circles
    public class gp_Circ
    {
    }
    }