namespace TKMath
{
    //! Describes a coordinate system in a plane (2D space).
    //! A coordinate system is defined by:
    //! -   its origin (also referred to as its "Location point"), and
    //! -   two orthogonal unit vectors, respectively, called the "X
    //! Direction" and the "Y Direction".
    //! A gp_Ax22d may be right-handed ("direct sense") or
    //! left-handed ("inverse" or "indirect sense").
    //! You use a gp_Ax22d to:
    //! - describe 2D geometric entities, in particular to position
    //! them. The local coordinate system of a geometric
    //! entity serves for the same purpose as the STEP
    //! function "axis placement two axes", or
    //! -   define geometric transformations.
    //! Note: we refer to the "X Axis" and "Y Axis" as the axes having:
    //! -   the origin of the coordinate system as their origin, and
    //! -   the unit vectors "X Direction" and "Y Direction",
    //! respectively, as their unit vectors.
    public class gp_Ax22d
    {

        gp_Pnt2d point;
        gp_Dir2d vydir;
        gp_Dir2d vxdir;
        //! Creates -   a coordinate system where its origin is the origin of
        //! theA and its "X Direction" is the unit vector of theA, which   is:
        //! -   right-handed if theIsSense is true (default value), or
        //! -   left-handed if theIsSense is false.
        public gp_Ax22d(gp_Ax2d theA, bool theIsSense = true)
        {
            point = (theA.Location());
            vxdir = (theA.Direction());


            if (theIsSense)
            {
                vydir.SetCoord(-vxdir.Y(), vxdir.X());
            }
            else
            {
                vydir.SetCoord(vxdir.Y(), -vxdir.X());
            }
        }
    }
}