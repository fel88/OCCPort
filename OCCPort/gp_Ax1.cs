using System;

namespace OCCPort
{
    public struct gp_Ax1
    {

        gp_Pnt loc;
        gp_Dir vdir;
        //! P is the location point and V is the direction of <me>.
        public gp_Ax1(gp_Pnt theP, gp_Dir theV)
        {

            loc = (theP);
            vdir = (theV);
        }

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