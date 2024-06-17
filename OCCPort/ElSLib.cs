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