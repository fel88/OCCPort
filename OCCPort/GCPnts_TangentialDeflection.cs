using OpenTK.Graphics.ES11;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Security.Cryptography;

namespace OCCPort
{
    //! Computes a set of  points on a curve from package
    //! Adaptor3d  such  as between  two successive   points
    //! P1(u1)and P2(u2) :
    //! @code
    //! . ||P1P3^P3P2||/||P1P3||*||P3P2||<AngularDeflection
    //! . ||P1P2^P1P3||/||P1P2||<CurvatureDeflection
    //! @endcode
    //! where P3 is the point of abscissa ((u1+u2)/2), with
    //! u1 the abscissa of the point P1 and u2 the abscissa
    //! of the point P2.
    //!
    //! ^ is the cross product of two vectors, and ||P1P2||
    //! the magnitude of the vector P1P2.
    //!
    //! The conditions AngularDeflection > gp::Resolution()
    //! and CurvatureDeflection > gp::Resolution() must be
    //! satisfied at the construction time.
    //!
    //! A minimum number of points can be fixed for a linear or circular element.
    //! Example:
    //! @code
    //! Handle(Geom_BezierCurve) aCurve = new Geom_BezierCurve (thePoles);
    //! GeomAdaptor_Curve aCurveAdaptor (aCurve);
    //! double aCDeflect  = 0.01; // Curvature deflection
    //! double anADeflect = 0.09; // Angular   deflection
    //!
    //! GCPnts_TangentialDeflection aPointsOnCurve;
    //! aPointsOnCurve.Initialize (aCurveAdaptor, anADeflect, aCDeflect);
    //! for (int i = 1; i <= aPointsOnCurve.NbPoints(); ++i)
    //! {
    //!   double aU   = aPointsOnCurve.Parameter (i);
    //!   gp_Pnt aPnt = aPointsOnCurve.Value (i);
    //! }
    //! @endcode
    public class GCPnts_TangentialDeflection
    {

        public GCPnts_TangentialDeflection()
        {
            myAngularDeflection = (0.0);
            myCurvatureDeflection = (0.0);
            myUTol = (0.0);
            myMinNbPnts = (0);
            myMinLen = (0.0);
            myLastU = (0.0);
            myFirstu = 0.0;

        }
        public gp_Pnt Value(int I)
        {
            return myPoints.Value(I);
        }

        public GCPnts_TangentialDeflection(Adaptor3d_Curve theC,
                                                          double theFirstParameter,
                                                          double theLastParameter,
                                                          double theAngularDeflection,
                                                          double theCurvatureDeflection,
                                                          int theMinimumOfPoints,
                                                          double theUTol,
                                                          double theMinLen)
        {
            myAngularDeflection = 0.0;
            myCurvatureDeflection = (0.0);
            myUTol = (0.0);
            myMinNbPnts = (0);
            myMinLen = (0.0);
            myLastU = (0.0);
            myFirstu = 0.0;


            Initialize(theC, theFirstParameter, theLastParameter,
                        theAngularDeflection, theCurvatureDeflection,
                        theMinimumOfPoints,
                        theUTol, theMinLen);
        }

        public void Initialize(Adaptor3d_Curve theC,
                                               double theFirstParameter,
                                               double theLastParameter,
                                               double theAngularDeflection,
             double theCurvatureDeflection,
                                               int theMinimumOfPoints,
                                               double theUTol,
                                               double theMinLen)
        {
            initialize(theC, theFirstParameter, theLastParameter,
                        theAngularDeflection, theCurvatureDeflection,
                        theMinimumOfPoints,
                        theUTol,
                        theMinLen);
        }

        double myAngularDeflection;
        double myCurvatureDeflection;
        double myUTol;
        int myMinNbPnts;
        double myMinLen;
        double myLastU;
        double myFirstu;
        public void initialize(dynamic theC,
                                              double theFirstParameter,
                                              double theLastParameter,
                                              double theAngularDeflection,
                                              double theCurvatureDeflection,
                                              int theMinimumOfPoints,
                                              double theUTol,
                                              double theMinLen)
        {
            Exceptions.Standard_ConstructionError_Raise_if(theCurvatureDeflection < Precision.Confusion() || theAngularDeflection < Precision.Angular(),
                                                 "GCPnts_TangentialDeflection::Initialize - Zero Deflection");
            myParameters.Clear();
            myPoints.Clear();
            if (theFirstParameter < theLastParameter)
            {
                myFirstu = theFirstParameter;
                myLastU = theLastParameter;
            }
            else
            {
                myLastU = theFirstParameter;
                myFirstu = theLastParameter;
            }
            myUTol = theUTol;
            myAngularDeflection = theAngularDeflection;
            myCurvatureDeflection = theCurvatureDeflection;
            myMinNbPnts = Math.Max(theMinimumOfPoints, 2);
            myMinLen = Math.Max(theMinLen, Precision.Confusion());

            switch (theC._GetType())
            {
                case GeomAbs_CurveType.GeomAbs_Line:
                    {
                        PerformLinear(theC);
                        break;
                    }
                //case GeomAbs_Circle:
                //    {
                //        PerformCircular(theC);
                //        break;
                //    }
                //case GeomAbs_BSplineCurve:
                //    {
                //        Handle(typename GCPnts_TCurveTypes < TheCurve >::BSplineCurve) aBS = theC.BSpline();
                //        if (aBS->NbPoles() == 2) PerformLinear(theC);
                //        else PerformCurve(theC);
                //        break;
                //    }
                //case GeomAbs_BezierCurve:
                //    {
                //        Handle(typename GCPnts_TCurveTypes < TheCurve >::BezierCurve) aBZ = theC.Bezier();
                //        if (aBZ->NbPoles() == 2) PerformLinear(theC);
                //        else PerformCurve(theC);
                //        break;
                //    }
                default:
                    {
                        PerformCurve(theC);
                        break;
                    }
            }
        }

        void PerformCurve(dynamic theC)
        {
        }

        public static void D0(Adaptor3d_Curve C, double U, ref gp_Pnt P)
        {
            C.D0(U, ref P);
        }
        public static void D0(Adaptor2d_Curve2d C, double U, ref gp_Pnt PP)
        {
            double X = 0, Y = 0;
            gp_Pnt2d P = new gp_Pnt2d();
            C.D0(U, ref P);
            P.Coord(ref X, ref Y);
            PP.SetCoord(X, Y, 0.0);
        }
        public void PerformLinear(Adaptor2d_Curve2d theC)
        {
            gp_Pnt P = new gp_Pnt();
            D0(theC, myFirstu, ref P);
            myParameters.Append(myFirstu);
            myPoints.Append(P);
            if (myMinNbPnts > 2)
            {
                double Du = (myLastU - myFirstu) / myMinNbPnts;
                double U = myFirstu + Du;
                for (int i = 2; i < myMinNbPnts; i++)
                {
                    D0(theC, U, ref P);
                    myParameters.Append(U);
                    myPoints.Append(P);
                    U += Du;
                }
            }
            D0(theC, myLastU, ref P);
            myParameters.Append(myLastU);
            myPoints.Append(P);
        }
        public void PerformLinear(Adaptor3d_Curve theC)
        {
            gp_Pnt P = new gp_Pnt();
            D0(theC, myFirstu, ref P);
            myParameters.Append(myFirstu);
            myPoints.Append(P);
            if (myMinNbPnts > 2)
            {
                double Du = (myLastU - myFirstu) / myMinNbPnts;
                double U = myFirstu + Du;
                for (int i = 2; i < myMinNbPnts; i++)
                {
                    D0(theC, U, ref P);
                    myParameters.Append(U);
                    myPoints.Append(P);
                    U += Du;
                }
            }
            D0(theC, myLastU, ref P);
            myParameters.Append(myLastU);
            myPoints.Append(P);
        }


        TColgp_SequenceOfPnt myPoints = new TColgp_SequenceOfPnt();
        TColStd_SequenceOfReal myParameters = new TColStd_SequenceOfReal();
        public int AddPoint
 (gp_Pnt thePnt,
  double theParam,
  bool theIsReplace)
        {
            double tol = Precision.PConfusion();
            int index = -1;
            int nb = myParameters.Length();
            for (int i = 1; index == -1 && i <= nb; i++)
            {
                double dist = myParameters.Value(i) - theParam;
                if (Math.Abs(dist) <= tol)
                {
                    index = i;
                    if (theIsReplace)
                    {
                        myPoints.ChangeValue(i, thePnt); ;
                        myParameters.ChangeValue(i, theParam);
                    }
                }
                else if (dist > tol)
                {
                    myPoints.InsertBefore(i, thePnt);
                    myParameters.InsertBefore(i, theParam);
                    index = i;
                }
            }
            if (index == -1)
            {
                myPoints.Append(thePnt);
                myParameters.Append(theParam);
                index = myParameters.Length();
            }
            return index;
        }
        public int NbPoints()
        {
            return myParameters.Length();
        }
        public double Parameter(int I)
        {
            return myParameters.Value(I);
        }
    }
}