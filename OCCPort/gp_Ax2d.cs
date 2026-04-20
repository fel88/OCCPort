namespace OCCPort
{
    //! Describes an axis in the plane (2D space).
    //! An axis is defined by:
    //! -   its origin (also referred to as its "Location point"),   and
    //! -   its unit vector (referred to as its "Direction").
    //! An axis implicitly defines a direct, right-handed
    //! coordinate system in 2D space by:
    //! -   its origin,
    //! - its "Direction" (giving the "X Direction" of the coordinate system), and
    //! -   the unit vector normal to "Direction" (positive angle
    //! measured in the trigonometric sense).
    //! An axis is used:
    //! -   to describe 2D geometric entities (for example, the
    //! axis which defines angular coordinates on a circle).
    //! It serves for the same purpose as the STEP function
    //! "axis placement one axis", or
    //! -   to define geometric transformations (axis of
    //! symmetry, axis of rotation, and so on).
    //! Note: to define a left-handed 2D coordinate system, use gp_Ax22d.
    class gp_Ax2d
    {

        //! Returns the origin of <me>.
        public gp_Pnt2d Location() { return loc; }

        //! Returns the direction of <me>.
        public gp_Dir2d Direction() { return vdir; }

        gp_Pnt2d loc;
        gp_Dir2d vdir;

    }
}