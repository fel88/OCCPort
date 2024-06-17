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



        public gp_Dir Direction()
        {
            return vdir;
        }

        internal gp_Pnt Location()
        {
            return loc;
        }
    }
}