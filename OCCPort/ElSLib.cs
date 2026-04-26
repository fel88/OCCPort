using System;

namespace OCCPort
{
    internal class ElSLib
    {

        public static void Parameters(gp_Pln Pl,
                    gp_Pnt P,
                   ref double U,
                 ref double V)
        {

            PlaneParameters(Pl.Position(), P, ref U, ref V);

        }

        internal static void PlaneD1(double U, double V, gp_Ax3 Pos, ref gp_Pnt P, ref gp_Vec Vu, ref gp_Vec Vv)
        {
            gp_XYZ XDir = Pos.XDirection().XYZ();
            gp_XYZ YDir = Pos.YDirection().XYZ();
            gp_XYZ PLoc = Pos.Location().XYZ();
            P.SetX(U * XDir.X() + V * YDir.X() + PLoc.X());
            P.SetY(U * XDir.Y() + V * YDir.Y() + PLoc.Y());
            P.SetZ(U * XDir.Z() + V * YDir.Z() + PLoc.Z());
            Vu.SetX(XDir.X());
            Vu.SetY(XDir.Y());
            Vu.SetZ(XDir.Z());
            Vv.SetX(YDir.X());
            Vv.SetY(YDir.Y());
            Vv.SetZ(YDir.Z());
        }

        internal static gp_Pnt PlaneValue(double U, double V, gp_Ax3 Pos)
        {
            gp_XYZ XDir = Pos.XDirection().XYZ();
            gp_XYZ YDir = Pos.YDirection().XYZ();
            gp_XYZ PLoc = Pos.Location().XYZ();
            return new gp_Pnt(U * XDir.X() + V * YDir.X() + PLoc.X(),
                  U * XDir.Y() + V * YDir.Y() + PLoc.Y(),
                  U * XDir.Z() + V * YDir.Z() + PLoc.Z());
        }

        //=======================================================================

        static void PlaneParameters(gp_Ax3 Pos,
                   gp_Pnt P,
                  ref double U,
                 ref double V)
        {
            gp_Trsf T = new gp_Trsf();
            T.SetTransformation(Pos);
            gp_Pnt Ploc = P.Transformed(T);
            U = Ploc.X();
            V = Ploc.Y();
        }



    }
}