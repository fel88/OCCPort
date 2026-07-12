using OCCPort.Common;

namespace TKMath
{
    //! Describes an axis in 3D space.
    //! An axis is defined by:
    //! -   its origin (also referred to as its "Location point"), and
    //! -   its unit vector (referred to as its "Direction" or "main   Direction").
    //! An axis is used:
    //! -   to describe 3D geometric entities (for example, the
    //! axis of a revolution entity). It serves the same purpose
    //! as the STEP function "axis placement one axis", or
    //! -   to define geometric transformations (axis of
    //! symmetry, axis of rotation, and so on).
    //! For example, this entity can be used to locate a geometric entity
    //! or to define a symmetry axis.
    public struct gp_Ax1
    {
        //! Assigns V as the "Direction"  of this axis.
        public void SetDirection(gp_Dir theV) { vdir = theV; }

        //! Assigns  P as the origin of this axis.
        public void SetLocation(gp_Pnt theP) { loc = theP; }
        //! Rotates this axis at an angle theAngRad (in radians) about the axis theA1
        //! and assigns the result to this axis.
        public void Rotate(gp_Ax1 theA1, double theAngRad)
        {
            loc.Rotate(theA1, theAngRad);
            vdir.Rotate(theA1, theAngRad);
        }

        gp_Pnt loc;
        gp_Dir vdir;
        //! P is the location point and V is the direction of <me>.
        public gp_Ax1(gp_Pnt theP, gp_Dir theV)
        {

            loc = (theP);
            vdir = (theV);
        }

        //! Translates this axis by the vector theV, and assigns the result to this axis.
        public void Translate(gp_Vec theV) { loc.Translate(theV); }

        //! Applies the transformation theT to this axis and assigns the result to this axis.
        public void Transform(gp_Trsf theT)
        {
            loc.Transform(theT);
            vdir.Transform(theT);
        }


        public gp_Dir Direction()
        {
            return vdir;
        }

        internal gp_Pnt Location()
        {
            return loc;
        }
        //! Reverses the unit vector of this axis and assigns the result to this axis.

        public void Reverse() => vdir.Reverse();
    }
}
