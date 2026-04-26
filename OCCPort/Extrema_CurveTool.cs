using System;

namespace OCCPort
{
    public class Extrema_CurveTool
    {
        public Extrema_CurveTool()
        {
        }

        public static GeomAbs_CurveType _GetType(Adaptor3d_Curve C)
        {
            return C._GetType();
        }

        internal static double Resolution(Adaptor3d_Curve C, double R3d)
        {
            return C.Resolution(R3d);

        }

        public static gp_Pnt Value(Adaptor3d_Curve C, double U)
        {
            return C.Value(U);
        }

        public static gp_Lin Line(Adaptor3d_Curve C)
        {
            return C.Line();
        }

        internal static bool IsPeriodic(Adaptor3d_Curve C)
        {
            GeomAbs_CurveType aType = _GetType(C);
            if (aType == GeomAbs_CurveType.GeomAbs_Circle ||
                aType == GeomAbs_CurveType.GeomAbs_Ellipse)
                return true;
            else
                return C.IsPeriodic();
        }

        internal static double Period(Adaptor3d_Curve aCurve)
        {
            throw new NotImplementedException();
        }
    }
}