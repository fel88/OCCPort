using OCCPort;
using System;
using System.Reflection.Metadata;
using System.Security.AccessControl;
using System.Security.Cryptography;

namespace OCCPort
{
    //! This class provides an interface between the services provided by any
    //! curve from the package Geom and those required of the curve by algorithms which use it.
    //! Creation of the loaded curve the curve is C1 by piece.
    //!
    //! Polynomial coefficients of BSpline curves used for their evaluation are
    //! cached for better performance. Therefore these evaluations are not
    //! thread-safe and parallel evaluations need to be prevented.
    public class GeomAdaptor_Curve : Adaptor3d_Curve
    {
        public GeomAdaptor_Curve()
        {
            myTypeCurve = GeomAbs_CurveType.GeomAbs_OtherCurve;
            myFirst = 0;
            myLast = 0;
        }

        public GeomAdaptor_Curve(Geom_Curve theCurve)
        {
            Load(theCurve);
        }

        public override double FirstParameter()
        {
            return myFirst;
        }

        public override double LastParameter()
        {
            return myLast;
        }

        public override gp_Lin Line()
        {
            Standard_NoSuchObject_Raise_if(myTypeCurve != GeomAbs_CurveType.GeomAbs_Line,
                                            "GeomAdaptor_Curve::Line() - curve is not a Line");
            return ((Geom_Line)myCurve).Lin();
        }



        private void Standard_NoSuchObject_Raise_if(bool v1, string v2)
        {
            if (v1)
                throw new Exception(v2);
        }

        public override GeomAbs_CurveType _GetType() { return myTypeCurve; }

        //! Standard_ConstructionError is raised if theUFirst>theULast
        public GeomAdaptor_Curve(Geom_Curve theCurve,
                      double theUFirst,
                      double theULast)
        {
            Load(theCurve, theUFirst, theULast);
        }

        GeomAbs_CurveType myTypeCurve;

        //! Standard_ConstructionError is raised if theUFirst>theULast
        public void Load(Geom_Curve theCurve,
             double theUFirst,
             double theULast)
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

        public void Load(Geom_Curve theCurve)
        {
            if (theCurve == null)
            {
                throw new Standard_NullObject();
            }
            load(theCurve, theCurve.FirstParameter(), theCurve.LastParameter());
        }

        double myFirst;
        double myLast;
        Geom_Curve myCurve;
        GeomEvaluator_Curve myNestedEvaluator; ///< Calculates value of offset curve
		Geom_BSplineCurve myBSplineCurve; ///< B-spline representation to prevent castings

        public void load(Geom_Curve C,
                             double UFirst,
                             double ULast)
        {
            myFirst = UFirst;
            myLast = ULast;
            //myCurveCache.Nullify();

            if (myCurve != C)
            {
                myCurve = C;
                myNestedEvaluator = null;
                myBSplineCurve = null;

                var TheType = C.GetType();
                if (TheType == typeof(Geom_TrimmedCurve))
                {
                    Load((C as Geom_TrimmedCurve).BasisCurve(), UFirst, ULast);
                }
                else
                /*if (TheType == typeof(Geom_Circle))
                {
                    myTypeCurve = GeomAbs_CurveType.GeomAbs_Circle;
                }
                else*/
                if (TheType == typeof(Geom_Line))
                {
                    myTypeCurve = GeomAbs_CurveType.GeomAbs_Line;
                }
                //else if (TheType == STANDARD_TYPE(Geom_Ellipse))
                //{
                //	myTypeCurve = GeomAbs_CurveType.GeomAbs_Ellipse;
                //}
                //else if (TheType == STANDARD_TYPE(Geom_Parabola))
                //{
                //	myTypeCurve = GeomAbs_CurveType.GeomAbs_Parabola;
                //}
                //else if (TheType == STANDARD_TYPE(Geom_Hyperbola))
                //{
                //	myTypeCurve = GeomAbs_CurveType.GeomAbs_Hyperbola;
                //}
                //else if (TheType == STANDARD_TYPE(Geom_BezierCurve))
                //{
                //	myTypeCurve = GeomAbs_CurveType.GeomAbs_BezierCurve;
                //}
                //else if (TheType == STANDARD_TYPE(Geom_BSplineCurve))
                //{
                //	myTypeCurve = GeomAbs_CurveType.GeomAbs_BSplineCurve;
                //	myBSplineCurve = Handle(Geom_BSplineCurve)::DownCast(myCurve);
                //}
                //else if (TheType == STANDARD_TYPE(Geom_OffsetCurve))
                //{
                //	myTypeCurve = GeomAbs_CurveType.GeomAbs_OffsetCurve;
                //	Geom_OffsetCurve anOffsetCurve = Handle(Geom_OffsetCurve)::DownCast(myCurve);
                //	// Create nested adaptor for base curve
                //	Geom_Curve aBaseCurve = anOffsetCurve.BasisCurve();
                //	GeomAdaptor_Curve aBaseAdaptor = new GeomAdaptor_Curve(aBaseCurve);
                //	myNestedEvaluator = new GeomEvaluator_OffsetCurve(
                //		aBaseAdaptor, anOffsetCurve.Offset(), anOffsetCurve.Direction());
                //}
                else
                {
                    myTypeCurve = GeomAbs_CurveType.GeomAbs_OtherCurve;
                }
            }
        }

        public override int Degree()
        {
            throw new NotImplementedException();
        }

        public override int NbKnots()
        {
            throw new NotImplementedException();
        }

        public override Geom_BSplineCurve BSpline()
        {
            throw new NotImplementedException();
        }

        public override gp_Pnt Value(double U)
        {
            gp_Pnt aValue = new gp_Pnt();
            D0(U, ref aValue);
            return aValue;
        }

        BSplCLib_Cache myCurveCache; ///< Cached data for B-spline or Bezier curve


        public void RebuildCache(double theParameter)
        {
            if (myTypeCurve == GeomAbs_CurveType.GeomAbs_BezierCurve)
            {
                // Create cache for Bezier
                Geom_BezierCurve aBezier = (Geom_BezierCurve)myCurve;
                //int aDeg = aBezier.Degree();
                //TColStd_Array1OfReal aFlatKnots(BSplCLib.FlatBezierKnots(aDeg), 1, 2 * (aDeg + 1));
                //if (myCurveCache == null)
                //    myCurveCache = new BSplCLib_Cache(aDeg, aBezier->IsPeriodic(), aFlatKnots,
                //      aBezier->Poles(), aBezier->Weights());
                //myCurveCache->BuildCache(theParameter, aFlatKnots, aBezier->Poles(), aBezier->Weights());
            }
            else if (myTypeCurve == GeomAbs_CurveType.GeomAbs_BSplineCurve)
            {
                // Create cache for B-spline
                //if (myCurveCache == null)
                //    myCurveCache = new BSplCLib_Cache(myBSplineCurve->Degree(), myBSplineCurve->IsPeriodic(),
                //      myBSplineCurve->KnotSequence(), myBSplineCurve->Poles(), myBSplineCurve->Weights());
                //myCurveCache->BuildCache(theParameter, myBSplineCurve->KnotSequence(),
                //                          myBSplineCurve->Poles(), myBSplineCurve->Weights());
            }
        }
        public override void D0(double U, ref gp_Pnt P)
        {
            switch (myTypeCurve)
            {
                case GeomAbs_CurveType.GeomAbs_BezierCurve:
                case GeomAbs_CurveType.GeomAbs_BSplineCurve:
                    {
                        int aStart = 0, aFinish = 0;
                        /*if (IsBoundary(U, aStart, aFinish))
                        {
                            myBSplineCurve.LocalD0(U, aStart, aFinish, P);
                        }
                        else*/
                        {
                            // use cached data
                            //if (myCurveCache == null || !myCurveCache.IsCacheValid(U))
                                RebuildCache(U);
                            myCurveCache.D0(U, ref P);
                        }
                        break;
                    }

                case GeomAbs_CurveType.GeomAbs_OffsetCurve:
                    //myNestedEvaluator.D0(U, ref P);
                    break;

                default:
                    myCurve.D0(U, ref P);
                    break;
            }
        }

        public override double Resolution(double R3d)
        {
            throw new NotImplementedException();
        }

        public override bool IsPeriodic()
        {
            throw new NotImplementedException();
        }

        public override int NbIntervals(GeomAbs_Shape S)
        {
            throw new NotImplementedException();
        }
    }
}