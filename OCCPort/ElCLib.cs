using System;

namespace OCCPort
{
    //! Provides functions for basic geometric computations on
    //! elementary curves such as conics and lines in 2D and 3D space.
    //! This includes:
    //! -   calculation of a point or derived vector on a 2D or
    //! 3D curve where:
    //! -   the curve is provided by the gp package, or
    //! defined in reference form (as in the gp package),
    //! and
    //! -   the point is defined by a parameter,
    //! -   evaluation of the parameter corresponding to a point
    //! on a 2D or 3D curve from gp,
    //! -   various elementary computations which allow you to
    //! position parameterized values within the period of a curve.
    //! Notes:
    //! -   ElCLib stands for Elementary Curves Library.
    //! -   If the curves provided by the gp package are not
    //! explicitly parameterized, they still have an implicit
    //! parameterization, analogous to that which they infer
    //! for the equivalent Geom or Geom2d curves.
    public class ElCLib
    {
        internal static double Parameter(gp_Lin L, gp_Pnt P)
        {
            return LineParameter(L.Position(), P);
        }

        //! Adjust U1 and  U2 in the  parametric range  UFirst
        //! Ulast of a periodic curve, where ULast -
        //! UFirst is its period. To do this, this function:
        //! -   sets U1 in the range [ UFirst, ULast ] by
        //! adding/removing the period to/from the value U1, then
        //! -   sets U2 in the range [ U1, U1 + period ] by
        //! adding/removing the period to/from the value U2.
        //! Precision is used to test the equalities.
        public static void AdjustPeriodic(double UFirst,
            double ULast, double Preci, ref double U1, ref double U2)
        {
            if (Precision.IsInfinite(UFirst) ||
     Precision.IsInfinite(ULast))
            {
                U1 = UFirst;
                U2 = ULast;
                return;
            }

            double period = ULast - UFirst;

            //if (period < Epsilon(ULast)) //todo: fix here to origin
            if (Math.Abs(period) < double.Epsilon) //todo: fix here to origin
            {
                // In order to avoid FLT_Overflow exception
                // (test bugs moddata_1 bug22757)
                U1 = UFirst;
                U2 = ULast;
                return;
            }

            U1 -= Math.Floor((U1 - UFirst) / period) * period;
            if (ULast - U1 < Preci) U1 -= period;
            U2 -= Math.Floor((U2 - U1) / period) * period;
            if (U2 - U1 < Preci) U2 += period;
        }
        //-------------------------------------------------------------------
        // Epsilon : The function returns absolute value of difference
        //           between 'Value' and other nearest value of
        //           Standard_Real type.
        //           Nearest value is chosen in direction of infinity
        //           the same sign as 'Value'.
        //           If 'Value' is 0 then returns minimal positive value
        //           of Standard_Real type.
        //-------------------------------------------------------------------
        public static double Epsilon(double Value)
        {
            double aEpsilon;

            if (Value >= 0.0)
            {
                aEpsilon = NextAfter(Value, Standard_Real.RealLast()) - Value;
            }
            else
            {
                aEpsilon = Value - NextAfter(Value, Standard_Real.RealFirst());
            }
            return aEpsilon;
        }

        private static double NextAfter(double value1, object value2)
        {
            throw new NotImplementedException();
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

        internal static void LineD1(double U, gp_Ax2d Pos, ref gp_Pnt2d P, ref gp_Vec2d V1)
        {
            gp_XY Coord = Pos.Direction().XY();
            V1.SetXY(Coord);
            Coord.SetLinearForm(U, Coord, Pos.Location().XY());
            P.SetXY(Coord);
        }

        internal static gp_Pnt Value(double U, gp_Lin L)
        {
            return ElCLib.LineValue(U, L.Position());
        }

        internal static gp_Pnt Value(double u, gp_Circ myCirc)
        {
            throw new NotImplementedException();
            //return ElCLib.CircleValue(U, C.Position(), C.Radius());
        }
    }
}