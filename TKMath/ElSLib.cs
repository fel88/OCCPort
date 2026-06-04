namespace TKMath
{
    //! Provides functions for basic geometric computation on
    //! elementary surfaces.
    //! This includes:
    //! -   calculation of a point or derived vector on a surface
    //! where the surface is provided by the gp package, or
    //! defined in canonical form (as in the gp package), and
    //! the point is defined with a parameter,
    //! -   evaluation of the parameters corresponding to a
    //! point on an elementary surface from gp,
    //! -   calculation of isoparametric curves on an elementary
    //! surface defined in canonical form (as in the gp package).
    //! Notes:
    //! -   ElSLib stands for Elementary Surfaces Library.
    //! -   If the surfaces provided by the gp package are not
    //! explicitly parameterized, they still have an implicit
    //! parameterization, similar to that which they infer on
    //! the equivalent Geom surfaces.
    //! Note: ElSLib stands for Elementary Surfaces Library.
    public class ElSLib
    {

        public static void Parameters(gp_Pln Pl,
                    gp_Pnt P,
                   ref double U,
                 ref double V)
        {

            PlaneParameters(Pl.Position(), P, ref U, ref V);

        }

        public static void PlaneD1(double U, double V, gp_Ax3 Pos, ref gp_Pnt P, ref gp_Vec Vu, ref gp_Vec Vv)
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


        public static gp_Lin PlaneUIso(gp_Ax3 Pos,
              double U)
        {
            gp_Lin L = new gp_Lin(Pos.Location(), Pos.YDirection());
            gp_Vec Ve = new gp_Vec(Pos.XDirection());
            Ve *= U;
            L.Translate(Ve);
            return L;
        }

        public static gp_Pnt PlaneValue(double U, double V, gp_Ax3 Pos)
        {
            gp_XYZ XDir = Pos.XDirection().XYZ();
            gp_XYZ YDir = Pos.YDirection().XYZ();
            gp_XYZ PLoc = Pos.Location().XYZ();
            return new gp_Pnt(U * XDir.X() + V * YDir.X() + PLoc.X(),
                  U * XDir.Y() + V * YDir.Y() + PLoc.Y(),
                  U * XDir.Z() + V * YDir.Z() + PLoc.Z());
        }

        //=======================================================================

        public static void PlaneParameters(gp_Ax3 Pos,
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

        public static gp_Lin PlaneVIso(gp_Ax3 Pos, double V)
        {
            gp_Lin L = new gp_Lin(Pos.Location(), Pos.XDirection());
            gp_Vec Ve = new gp_Vec(Pos.YDirection());
            Ve *= V;
            L.Translate(Ve);
            return L;
        }
    }
}
