using OCCPort.Common;

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

        //! Creates a coordinate system with origin theP and where:
        //! -   theVx is the "X Direction", and
        //! -   the "Y Direction" is orthogonal to theVx and
        //! oriented so that the cross products theVx^"Y
        //! Direction" and theVx^theVy have the same sign.
        //! Raises ConstructionError if theVx and theVy are parallel (same or opposite orientation).
        public gp_Ax22d(gp_Pnt2d theP, gp_Dir2d theVx, gp_Dir2d theVy)
        {
            point = (theP);
            vydir = (theVy);
            vxdir = (theVx);

            double aValue = theVx.Crossed(theVy);
            if (aValue >= 0.0)
            {
                vydir.SetCoord(-vxdir.Y(), vxdir.X());
            }
            else
            {
                vydir.SetCoord(vxdir.Y(), -vxdir.X());
            }
        }

        //! Returns an axis, for which
        //! -   the origin is that of this coordinate system, and
        //! -   the unit vector is either the "X Direction"  of this coordinate system.
        //! Note: the result is the "X Axis" of this coordinate system.
        public gp_Ax2d XAxis() { return new gp_Ax2d(point, vxdir); }

        //! Returns an axis, for which
        //! -   the origin is that of this coordinate system, and
        //! - the unit vector is either the  "Y Direction" of this coordinate system.
        //! Note: the result is the "Y Axis" of this coordinate system.
        public gp_Ax2d YAxis()
        {
            return new gp_Ax2d(point, vydir);
        }


        //! Returns the "Location" point (origin) of <me>.
        public gp_Pnt2d Location() { return point; }

        //! Returns the "XDirection" of <me>.
        public gp_Dir2d XDirection() { return vxdir; }

        //! Returns the "YDirection" of <me>.
        public gp_Dir2d YDirection() { return vydir; }

        gp_Pnt2d point = new gp_Pnt2d();
        gp_Dir2d vydir = new gp_Dir2d();
        gp_Dir2d vxdir = new gp_Dir2d();
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