using OCCPort;
using System;
using System.Reflection.Metadata;
using System.Security.AccessControl;
using System.Security.Cryptography;

namespace OCCPort
{
    //! An interface between the services provided by any
    //! curve from the package Geom2d and those required
    //! of the curve by algorithms which use it.
    //!
    //! Polynomial coefficients of BSpline curves used for their evaluation are
    //! cached for better performance. Therefore these evaluations are not
    //! thread-safe and parallel evaluations need to be prevented.
    public class Geom2dAdaptor_Curve : Adaptor2d_Curve2d
    {
        Geom2d_Curve myCurve;
        GeomAbs_CurveType myTypeCurve;
        double myFirst;
        double myLast;
        public override Geom2d_BezierCurve Bezier()
        {
            return (Geom2d_BezierCurve)myCurve;
        }
        public override Geom2d_BSplineCurve BSpline()
        {
            return myBSplineCurve;
        }
        Geom2d_BSplineCurve myBSplineCurve; ///< B-spline representation to prevent castings

        public gp_Pnt2d Value(double U)
        {
            gp_Pnt2d aRes = new gp_Pnt2d();
            D0(U, ref aRes);
            return aRes;
        }

        public override double FirstParameter() { return myFirst; }

        public override double LastParameter() { return myLast; }

        public void D0(double U, ref gp_Pnt2d P)
        {
            //switch (myTypeCurve)
            //{
            //case GeomAbs_BezierCurve:
            //case GeomAbs_BSplineCurve:
            //    {
            //        Standard_Integer aStart = 0, aFinish = 0;
            //        if (IsBoundary(U, aStart, aFinish))
            //        {
            //            myBSplineCurve->LocalD0(U, aStart, aFinish, P);
            //        }
            //        else
            //        {
            //            // use cached data
            //            if (myCurveCache.IsNull() || !myCurveCache->IsCacheValid(U))
            //                RebuildCache(U);
            //            myCurveCache->D0(U, P);
            //        }
            //        break;
            //    }

            //case GeomAbs_OffsetCurve:
            //    myNestedEvaluator->D0(U, P);
            //    break;

            //default:
            myCurve.D0(U, ref P);
            //}
        }
        public Geom2dAdaptor_Curve(Geom2d_Curve theCrv, double theUFirst, double theULast)
        {
            myTypeCurve = GeomAbs_CurveType.GeomAbs_OtherCurve;
            myFirst = theUFirst;
            myLast = theULast;

            Load(theCrv, theUFirst, theULast);
        }
        //! Standard_ConstructionError is raised if theUFirst>theULast
        public void Load(Geom2d_Curve theCurve, double theUFirst, double theULast)
        {
            if (theCurve == null)
            {
                throw new Standard_NullObject();
            }
            if (theUFirst > theULast)
            {
                throw new Standard_ConstructionError();
            }
            load(theCurve, theUFirst, theULast);
        }
        void load(Geom2d_Curve C,
                                     double UFirst,
                                     double ULast)
        {
            myFirst = UFirst;
            myLast = ULast;
            //myCurveCache.Nullify();

            if (myCurve != C)
            {
                myCurve = C;
                //myNestedEvaluator.Nullify();
                //myBSplineCurve.Nullify();

                Type TheType = C.GetType();
                //if (TheType == STANDARD_TYPE(Geom2d_TrimmedCurve))
                //{
                //    Load(Handle(Geom2d_TrimmedCurve)::DownCast(C)->BasisCurve(),
                //     UFirst, ULast);
                //}
                //else if (TheType == STANDARD_TYPE(Geom2d_Circle))
                //{
                //    myTypeCurve = GeomAbs_Circle;
                //}
                //else 
                if (C is Geom2d_Line)
                {
                    myTypeCurve = GeomAbs_CurveType.GeomAbs_Line;
                }
                //else if (TheType == STANDARD_TYPE(Geom2d_Ellipse))
                //{
                //    myTypeCurve = GeomAbs_Ellipse;
                //}
                //else if (TheType == STANDARD_TYPE(Geom2d_Parabola))
                //{
                //    myTypeCurve = GeomAbs_Parabola;
                //}
                //else if (TheType == STANDARD_TYPE(Geom2d_Hyperbola))
                //{
                //    myTypeCurve = GeomAbs_Hyperbola;
                //}
                //else if (TheType == STANDARD_TYPE(Geom2d_BezierCurve))
                //{
                //    myTypeCurve = GeomAbs_BezierCurve;
                //}
                else if (C is Geom2d_BSplineCurve)
                {
                    myTypeCurve = GeomAbs_CurveType.GeomAbs_BSplineCurve;
                    myBSplineCurve = (Geom2d_BSplineCurve)myCurve;
                }
                //else if (TheType == STANDARD_TYPE(Geom2d_OffsetCurve))
                //{
                //    myTypeCurve = GeomAbs_OffsetCurve;
                //    Handle(Geom2d_OffsetCurve) anOffsetCurve = Handle(Geom2d_OffsetCurve)::DownCast(myCurve);
                //    // Create nested adaptor for base curve
                //    Handle(Geom2d_Curve) aBaseCurve = anOffsetCurve->BasisCurve();
                //    Handle(Geom2dAdaptor_Curve) aBaseAdaptor = new Geom2dAdaptor_Curve(aBaseCurve);
                //    myNestedEvaluator = new Geom2dEvaluator_OffsetCurve(aBaseAdaptor, anOffsetCurve->Offset());
                //}
                else
                {
                    myTypeCurve = GeomAbs_CurveType.GeomAbs_OtherCurve;
                }
            }
        }

        public override int NbSamples()
        {
            return nbPoints(myCurve);
        }
        public static int nbPoints(Geom2d_Curve theCurve)
        {

            int nbs = 20;

            if (theCurve is Geom2d_Line)
                nbs = 2;
            //else if (theCurve->IsKind(STANDARD_TYPE(Geom2d_BezierCurve)))
            //{
            //    nbs = 3 + Handle(Geom2d_BezierCurve)::DownCast(theCurve)->NbPoles();
            //}
            else if (theCurve is Geom2d_BSplineCurve)
            {
                nbs = ((Geom2d_BSplineCurve)theCurve).NbKnots();
                nbs *= ((Geom2d_BSplineCurve)theCurve).Degree();
                if (nbs < 2.0) nbs = 2;
            }
            //else if (theCurve->IsKind(STANDARD_TYPE(Geom2d_OffsetCurve)))
            //{
            //    Handle(Geom2d_Curve) aCurve = Handle(Geom2d_OffsetCurve)::DownCast(theCurve)->BasisCurve();
            //    return Max(nbs, nbPoints(aCurve));
            //}

            //else if (theCurve->IsKind(STANDARD_TYPE(Geom2d_TrimmedCurve)))
            //{
            //    Handle(Geom2d_Curve) aCurve = Handle(Geom2d_TrimmedCurve)::DownCast(theCurve)->BasisCurve();
            //    return Max(nbs, nbPoints(aCurve));
            //}
            if (nbs > 300)
                nbs = 300;
            return nbs;

        }

        public override GeomAbs_CurveType _GetType()
        {
            throw new NotImplementedException();
        }

        public override gp_Lin2d Line()
        {
            Exceptions.Standard_NoSuchObject_Raise_if(myTypeCurve != GeomAbs_CurveType.GeomAbs_Line,
                                  "Geom2dAdaptor_Curve::Line() - curve is not a Line");
            return ((Geom2d_Line)myCurve).Lin2d();
        }



        public override int Degree()
        {
            throw new NotImplementedException();
        }

        public override int NbKnots()
        {
            throw new NotImplementedException();
        }
    }

}