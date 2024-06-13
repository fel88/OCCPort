using System;

namespace OCCPort
{
    public struct gp_Ax1
    {       

        gp_Pnt loc;
        gp_Dir vdir;

        public gp_Ax1(gp_Pnt aRCenter, gp_Dir aYAxis)
        {
            this.loc = aRCenter;
            this.vdir = aYAxis;
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