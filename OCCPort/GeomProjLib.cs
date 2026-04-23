using System;
using System.Numerics;
using System.Reflection.Metadata;
using System.Security.Cryptography;

namespace OCCPort
{
    internal class GeomProjLib
    {

        public static Geom_Curve ProjectOnPlane(Geom_Curve Curve,
 Geom_Plane Plane,
 gp_Dir Dir,
 bool KeepParametrization)
        {
            GeomAdaptor_Curve AC = new GeomAdaptor_Curve(Curve);
            GeomAdaptor_Curve HC = (AC);

            ProjLib_ProjectOnPlane Proj = new ProjLib_ProjectOnPlane(Plane.Position(), Dir);
            Proj.Load(HC, Precision.Approximation(), KeepParametrization);

            Geom_Curve GC = null;

            switch (Proj.GetType())
            {
                case GeomAbs_CurveType.GeomAbs_Line:
                    GC = new Geom_Line(Proj.Line());
                    break;

                //case GeomAbs_CurveType.GeomAbs_Circle:
                //    GC = new Geom_Circle(Proj.Circle());
                //    break;

                //case GeomAbs_CurveType.GeomAbs_Ellipse:
                //    GC = new Geom_Ellipse(Proj.Ellipse());
                //    break;

                //case GeomAbs_CurveType.GeomAbs_Parabola:
                //    GC = new Geom_Parabola(Proj.Parabola());
                //    break;

                //case GeomAbs_CurveType.GeomAbs_Hyperbola:
                //    GC = new Geom_Hyperbola(Proj.Hyperbola());
                //    break;

                //case GeomAbs_CurveType.GeomAbs_BezierCurve:
                //    GC = Proj.Bezier();
                //    break;

                //case GeomAbs_CurveType.GeomAbs_BSplineCurve:
                //    GC = Proj.BSpline();
                //    break;
                default:
                    return GC;

            }

            /*if (Curve.IsKind(STANDARD_TYPE(Geom_TrimmedCurve)))
            {
                Handle(Geom_TrimmedCurve) CTrim
                  = Handle(Geom_TrimmedCurve)::DownCast(Curve);
                GC = new Geom_TrimmedCurve(GC, Proj.FirstParameter(),
                                    Proj.LastParameter());
            }*/

            return GC;

        }
    }
}