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

        public gp_Pnt2d Value(double U)
        {
            gp_Pnt2d aRes = new gp_Pnt2d();
            D0(U, ref aRes);
            return aRes;
        }

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
                //else if (TheType == STANDARD_TYPE(Geom2d_BSplineCurve))
                //{
                //    myTypeCurve = GeomAbs_BSplineCurve;
                //    myBSplineCurve = Handle(Geom2d_BSplineCurve)::DownCast(myCurve);
                //}
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
        internal int NbSamples()
        {

            return 20;


        }


    }

}