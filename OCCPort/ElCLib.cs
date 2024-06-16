using System;

namespace OCCPort
{
    internal class ElCLib
    {
        internal static double Parameter(gp_Lin L, gp_Pnt P)
        {
            return LineParameter(L.Position(), P);
        }

        private static double LineParameter(gp_Ax1 L, gp_Pnt P)
        {
            return (P.XYZ() - L.Location().XYZ()).Dot(L.Direction().XYZ());
        }
    }
}