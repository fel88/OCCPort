using System;

namespace OCCPort
{
    public class ElCLib
    {
        internal static double Parameter(gp_Lin L, gp_Pnt P)
        {
            return LineParameter(L.Position(), P);
        }
        public static gp_Pnt LineValue(double U,
              gp_Ax1 Pos)
        {
            gp_XYZ ZDir = Pos.Direction().XYZ();
            gp_XYZ PLoc = Pos.Location().XYZ();
            return new gp_Pnt(U * ZDir.X() + PLoc.X(),
                  U * ZDir.Y() + PLoc.Y(),
                  U * ZDir.Z() + PLoc.Z());
        }

        private static double LineParameter(gp_Ax1 L, gp_Pnt P)
        {
            return (P.XYZ() - L.Location().XYZ()).Dot(L.Direction().XYZ());
        }

        internal static gp_Pnt2d LineValue(double U, gp_Ax2d Pos)
        {
            gp_XY ZDir = Pos.Direction().XY();
            gp_XY PLoc = Pos.Location().XY();
            return new gp_Pnt2d(U * ZDir.X() + PLoc.X(),
                    U * ZDir.Y() + PLoc.Y());

        }
    }
}