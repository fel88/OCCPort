using System;

namespace OCCPort
{
    internal class gp_Ax1
    {
        private gp_Pnt aRCenter;
        private gp_Dir aYAxis;

        public gp_Ax1(gp_Pnt aRCenter, gp_Dir aYAxis)
        {
            this.aRCenter = aRCenter;
            this.aYAxis = aYAxis;
        }

        internal gp_Dir Direction()
        {
            throw new NotImplementedException();
        }

        internal gp_Pnt Location()
        {
            throw new NotImplementedException();
        }
    }
}