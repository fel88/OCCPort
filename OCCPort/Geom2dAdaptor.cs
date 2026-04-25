using OCCPort;
using System;

namespace OCCPort
{//! this package  contains the geometric definition of
 //! 2d  curves compatible  with  the  Adaptor  package
 //! templates.
    internal class Geom2dAdaptor
    {

        //! Inherited  from    GHCurve.   Provides a  curve
        //! handled by reference.
        //! Creates  a 2d  curve  from  a  HCurve2d.  This
        //! cannot process the OtherCurves.
        public static Geom2d_Curve MakeCurve(Adaptor2d_Curve2d HC)
        {
            Geom2d_Curve C2D = null;
            switch (HC._GetType())
            {

                case GeomAbs_CurveType.GeomAbs_Line:
                    {
                        Geom2d_Line GL = new Geom2d_Line(HC.Line());
                        C2D = GL;
                    }
                    break;

                //case GeomAbs_Circle:
                //    {
                //        Handle(Geom2d_Circle) GL = new Geom2d_Circle(HC.Circle());
                //        C2D = GL;
                //    }
                //    break;


                //case GeomAbs_Ellipse:
                //    {
                //        Handle(Geom2d_Ellipse) GL = new Geom2d_Ellipse(HC.Ellipse());
                //        C2D = GL;
                //    }
                //    break;

                //case GeomAbs_Parabola:
                //    {
                //        Handle(Geom2d_Parabola) GL = new Geom2d_Parabola(HC.Parabola());
                //        C2D = GL;
                //    }
                //    break;

                //case GeomAbs_Hyperbola:
                //    {
                //        Handle(Geom2d_Hyperbola) GL = new Geom2d_Hyperbola(HC.Hyperbola());
                //        C2D = GL;
                //    }
                //    break;

                //case GeomAbs_BezierCurve:
                //    {
                //        C2D = HC.Bezier();
                //    }
                //    break;

                //case GeomAbs_BSplineCurve:
                //    {
                //        C2D = HC.BSpline();
                //    }
                //    break;

                //case GeomAbs_OffsetCurve:
                //    {
                //        const Geom2dAdaptor_Curve* pGAC = dynamic_cast <const Geom2dAdaptor_Curve*> (&HC);
                //        if (pGAC != 0)
                //        {
                //            C2D = pGAC->Curve();
                //        }
                //        else
                //        {
                //            Standard_DomainError::Raise("Geom2dAdaptor::MakeCurve, Not Geom2dAdaptor_Curve");
                //        }
                //    }
                //    break;

                default:
                    throw new Standard_DomainError("Geom2dAdaptor::MakeCurve, OtherCurve");

            }

            //trim the curve if necassary.
            //if (!C2D.IsNull() &&
            //    ((HC.FirstParameter() != C2D.FirstParameter()) ||
            //    (HC.LastParameter() != C2D.LastParameter())))
            //{

            //    if (C2D.IsPeriodic() ||
            //      (HC.FirstParameter() >= C2D.FirstParameter() &&
            //      HC.LastParameter() <= C2D.LastParameter()))
            //    {
            //        C2D = new Geom2d_TrimmedCurve
            //          (C2D, HC.FirstParameter(), HC.LastParameter());
            //    }
            //    else
            //    {
            //        double tf = Math.Max(HC.FirstParameter(), C2D.FirstParameter());
            //        double tl = Math.Min(HC.LastParameter(), C2D.LastParameter());
            //        C2D = new Geom2d_TrimmedCurve(C2D, tf, tl);
            //    }
            //}

            return C2D;
        }

    }
}