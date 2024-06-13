using System;

namespace OCCPort
{
    internal struct gp_Ax1
    {       

        gp_Pnt loc;
        gp_Dir vdir;

        public gp_Ax1(gp_Pnt aRCenter, gp_Dir aYAxis)
        {
            this.loc = aRCenter;
            this.vdir = aYAxis;
        }

        internal gp_Dir Direction()
        {
            return vdir;
        }

        internal gp_Pnt Location()
        {
            return loc;
        }
    }
}