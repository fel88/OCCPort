using OCCPort;
using System;
using System.Runtime.Intrinsics.X86;

namespace OCCPort.Tester
{
    //! Provides an  algorithm to find  a Surface  through a
    //! set of edges.
    //!
    //! The edges  of  the  shape  given  as  argument are
    //! explored if they are not coplanar at  the required
    //! tolerance  the method Found returns false.
    //!
    //! If a null tolerance is given the max of the  edges
    //! tolerances is used.
    //!
    //! The method Tolerance returns the true distance  of
    //! the edges to the Surface.
    //!
    //! The method Surface returns the Surface if found.
    //!
    //! The method Existed  returns returns  True  if  the
    //! Surface was already attached to some of the edges.
    //!
    //! When Existed  returns True  the  Surface  may have a
    //! location given by the Location method.
    public class BRepLib_FindSurface
    {
        public double Tolerance()
        {
            return myTolerance;
        }
        //! Computes the Surface from the edges of  <S> with the
        //! given tolerance.
        //! if <OnlyPlane> is true, the computed surface will be
        //! a plane. If it is not possible to find a plane, the
        //! flag NotDone will be set.
        //! If <OnlyClosed> is true,  then  S  should be a wire
        //! and the existing surface,  on  which wire S is not
        //! closed in 2D, will be ignored.
        public BRepLib_FindSurface(TopoDS_Shape S, double Tol = -1, bool OnlyPlane = false,
            bool OnlyClosed = false)
        {
            Init(S, Tol, OnlyPlane, OnlyClosed);
        }
        Geom_Surface mySurface;
        double myTolerance;
        double myTolReached;
        bool isExisted;
        TopLoc_Location myLocation;



        public void Init(TopoDS_Shape S,
                                     double Tol,

                                     bool OnlyPlane,
                               bool OnlyClosed)
        {
            myTolerance = Tol;
            myTolReached = 0.0;
            isExisted = false;
            myLocation.Identity();
            mySurface = null;

            // compute the tolerance
            TopExp_Explorer ex = new TopExp_Explorer();

            for (ex.Init(S, TopAbs_ShapeEnum.TopAbs_EDGE); ex.More(); ex.Next())
            {
                double t = BRep_Tool.Tolerance(TopoDS.Edge(ex.Current()));
                if (t > myTolerance)
                    myTolerance = t;

            }

            // search an existing surface
            ex.Init(S, TopAbs_ShapeEnum.TopAbs_EDGE);
            if (!ex.More()) return;    // no edges ....

            TopoDS_Edge E = TopoDS.Edge(ex.Current());
            double f, l, ff, ll;
            Geom2d_Curve PC, aPPC;
            Geom_Surface SS;
            TopLoc_Location L;
            int i = 0, j;

            // iterate on the surfaces of the first edge
            for (; ; )
            {
                i++;
                //BRep_Tool.CurveOnSurface(E, PC, mySurface, myLocation, f, l, i);
                if (mySurface == null)
                {
                    break;
                }
                // check the other edges
                for (ex.Init(S, TopAbs_ShapeEnum.TopAbs_EDGE); ex.More(); ex.Next())
                {
                    if (!E.IsSame(ex.Current()))
                    {
                        j = 0;
                        for (; ; )
                        {
                            j++;
                            //BRep_Tool::CurveOnSurface(TopoDS::Edge(ex.Current()), aPPC, SS, L, ff, ll, j);
                            //if (SS.IsNull())
                            //{
                            //    break;
                            //}
                            //if ((SS == mySurface) && (L.IsEqual(myLocation)))
                            //{
                            //    break;
                            //}
                            //SS.Nullify();
                        }

                        //if (SS.IsNull())
                        //{
                        //    mySurface.Nullify();
                        //    break;
                        //}
                    }
                }

                //// if OnlyPlane, eval if mySurface is a plane.
                //if (OnlyPlane && !mySurface.IsNull())
                //{
                //    if (mySurface->IsKind(STANDARD_TYPE(Geom_RectangularTrimmedSurface)))
                //        mySurface = Handle(Geom_RectangularTrimmedSurface)::DownCast(mySurface)->BasisSurface();
                //    mySurface = Handle(Geom_Plane)::DownCast(mySurface);
                //}

                //if (!mySurface.IsNull())
                //    // if S is e.g. the bottom face of a cylinder, mySurface can be the
                //    // lateral (cylindrical) face of the cylinder; reject an improper mySurface
                //    if (!OnlyClosed || Is2DClosed(S, mySurface, myLocation))
                //        break;
            }

            if (mySurface != null)
            {
                isExisted = true;
                return;
            }
            //
            // no existing surface, search a plane
            // 07/02/02 akm vvv : (OCC157) changed algorithm
            //                    1. Collect the points along all edges of the shape
            //                       For each point calculate the WEIGHT = sum of
            //                       distances from neighboring points (_only_ same edge)
            //                    2. Minimizing the weighed sum of squared deviations
            //                       compute coefficients of the sought plane.

            TColgp_SequenceOfPnt aPoints = new TColgp_SequenceOfPnt();
            TColStd_SequenceOfReal aWeight;

            // ======================= Step #1
            for (ex.Init(S, TopAbs_ShapeEnum.TopAbs_EDGE); ex.More(); ex.Next())
            {
                BRepAdaptor_Curve c = new BRepAdaptor_Curve(TopoDS.Edge(ex.Current()));

                double dfUf = c.FirstParameter();
                double dfUl = c.LastParameter();
                //if (IsEqual(dfUf, dfUl))
                //{
                //    // Degenerate
                //    continue;
                //}
                int iNbPoints = 0;

                // Fill the parameters of the sampling points
                NCollection_Vector<double> aParams = new NCollection_Vector<double>();
                switch (c._GetType())
                {
                    //case GeomAbs_BezierCurve:
                    //    {
                    //        Handle(Geom_BezierCurve) GC = c.Bezier();
                    //        TColStd_Array1OfReal aKnots(1, 2);
                    //        aKnots.SetValue(1, GC->FirstParameter());
                    //        aKnots.SetValue(2, GC->LastParameter());

                    //        fillParams(aKnots, GC->Degree(), dfUf, dfUl, aParams);
                    //        break;
                    //    }
                    //case GeomAbs_BSplineCurve:
                    //    {
                    //        Handle(Geom_BSplineCurve) GC = c.BSpline();
                    //        fillParams(GC->Knots(), GC->Degree(), dfUf, dfUl, aParams);
                    //        break;
                    //    }
                    case GeomAbs_CurveType.GeomAbs_Line:
                        {
                            // Two points on a straight segment
                            aParams.Append(dfUf);
                            aParams.Append(dfUl);
                            break;
                        }
                    //case GeomAbs_Circle:
                    //case GeomAbs_Ellipse:
                    //case GeomAbs_Hyperbola:
                    //case GeomAbs_Parabola:
                    //    // Four points on other analytical curves
                    //    iNbPoints = 4;
                    //    Standard_FALLTHROUGH
                    default:
                        {
                            // Put some points on other curves
                            if (iNbPoints == 0)
                                iNbPoints = 15 + c.NbIntervals(GeomAbs_Shape.GeomAbs_C3);

                            //  TColStd_Array1OfReal aBounds = new TColStd_Array1OfReal(1, 2);
                            // aBounds.SetValue(1, dfUf);
                            //  aBounds.SetValue(2, dfUl);

                            // fillParams(aBounds, iNbPoints - 1, dfUf, dfUl, aParams);
                            break;
                        }

                }

                // Add the points with weights to the sequences
                //fillPoints(c, aParams, aPoints, aWeight);
            }

            if (aPoints.Length() < 3)
            {
                return;
            }

            //// ======================= Step #2
            //myLocation.Identity();
            //int iPoint;
            //math_Matrix aMat = new math_Matrix(1, 3, 1, 3, 0.);
            //math_Vector aVec = new math_Vector(1, 3, 0.);
            //// Find the barycenter and normalize weights 
            //double dfMaxWeight = 0.;
            //gp_XYZ aBaryCenter = new gp_XYZ(0., 0., 0.);
            //double dfSumWeight = 0.;
            //for (iPoint = 1; iPoint <= aPoints.Length(); iPoint++)
            //{
            //    double dfW = aWeight(iPoint);
            //    aBaryCenter += dfW * aPoints(iPoint).XYZ();
            //    dfSumWeight += dfW;
            //    if (dfW > dfMaxWeight)
            //    {
            //        dfMaxWeight = dfW;
            //    }
            //}
            //aBaryCenter /= dfSumWeight;

            //// Fill the matrix and the right vector
            //for (iPoint = 1; iPoint <= aPoints.Length(); iPoint++)
            //{
            //    gp_XYZ p = aPoints[iPoint].XYZ() - aBaryCenter;
            //    double w = aWeight(iPoint) / dfMaxWeight;
            //    aMat[1, 1] += w * p.X() * p.X();
            //    aMat(1, 2) += w * p.X() * p.Y();
            //    aMat(1, 3) += w * p.X() * p.Z();
            //    //  
            //    aMat(2, 2) += w * p.Y() * p.Y();
            //    aMat(2, 3) += w * p.Y() * p.Z();
            //    //  
            //    aMat(3, 3) += w * p.Z() * p.Z();
            //}
            //aMat(2, 1) = aMat(1, 2);
            //aMat(3, 1) = aMat(1, 3);
            //aMat(3, 2) = aMat(2, 3);
            ////
            //math_Jacobi anEignval(aMat);
            //math_Vector anEVals(1,3);
            //Standard_Boolean isSolved = anEignval.IsDone();
            //Standard_Integer isol = 0;
            //if (isSolved)
            //{
            //    anEVals = anEignval.Values();
            //    //We need vector with eigenvalue ~ 0.
            //    Standard_Real anEMin = RealLast();
            //    Standard_Real anEMax = -anEMin;
            //    for (i = 1; i <= 3; ++i)
            //    {
            //        Standard_Real anE = Abs(anEVals(i));
            //        if (anEMin > anE)
            //        {
            //            anEMin = anE;
            //            isol = i;
            //        }
            //        if (anEMax < anE)
            //        {
            //            anEMax = anE;
            //        }
            //    }

            //    if (isol == 0)
            //    {
            //        isSolved = Standard_False;
            //    }
            //    else
            //    {
            //        Standard_Real eps = Epsilon(anEMax);
            //        if (anEMin <= eps)
            //        {
            //            anEignval.Vector(isol, aVec);
            //        }
            //        else
            //        {
            //            //try using vector product of other axes
            //            Standard_Integer ind[2] = { 0, 0 };
            //            for (i = 1; i <= 3; ++i)
            //            {
            //                if (i == isol)
            //                {
            //                    continue;
            //                }
            //                if (ind[0] == 0)
            //                {
            //                    ind[0] = i;
            //                    continue;
            //                }
            //                if (ind[1] == 0)
            //                {
            //                    ind[1] = i;
            //                    continue;
            //                }
            //            }
            //            math_Vector aVec1(1, 3, 0.), aVec2(1, 3, 0.);
            //            anEignval.Vector(ind[0], aVec1);
            //            anEignval.Vector(ind[1], aVec2);
            //            gp_Vec aV1(aVec1(1), aVec1(2), aVec1(3));
            //            gp_Vec aV2(aVec2(1), aVec2(2), aVec2(3));
            //            gp_Vec aN = aV1 ^ aV2;
            //            aVec(1) = aN.X();
            //            aVec(2) = aN.Y();
            //            aVec(3) = aN.Z();
            //        }
            //        if (aVec.Norm2() < gp::Resolution())
            //        {
            //            isSolved = Standard_False;
            //        }
            //    }
            //}

            //if (!isSolved)
            //    return;
            ////Removing very small values
            //double aMaxV = Max(Abs(aVec(1)), Max(Abs(aVec(2)), Abs(aVec(3))));
            //double eps = Epsilon(aMaxV);
            //for (i = 1; i <= 3; ++i)
            //{
            //    if (Abs(aVec(i)) <= eps)
            //        aVec(i) = 0.;
            //}
            //gp_Vec aN=new gp_Vec (aVec (1), aVec(2), aVec(3));
            //Geom_Plane aPlane = new Geom_Plane(aBaryCenter, aN);
            //myTolReached = Controle(aPoints, aPlane);
            double aWeakness = 5.0;
            //if (myTolReached <= myTolerance || (Tol < 0 && myTolReached < myTolerance * aWeakness))
            {
                // mySurface = aPlane;
                //If S is wire, try to orient surface according to orientation of wire.
                //if (S.ShapeType() == TopAbs_WIRE && S.Closed())
                {
                    //TopoDS_Wire aW = TopoDS::Wire(S);
                    //TopoDS_Face aTmpFace = BRepLib_MakeFace(mySurface, Precision::Confusion());
                    //BRep_Builder BB;
                    //BB.Add(aTmpFace, aW);
                    //BRepTopAdaptor_FClass2d FClass(aTmpFace, 0.);
                    //if (FClass.PerformInfinitePoint() == TopAbs_IN)
                    //{
                    //    gp_Dir aNorm = aPlane->Position().Direction();
                    //    aNorm.Reverse();
                    //    mySurface = new Geom_Plane(aPlane->Position().Location(), aNorm);
                    //}
                }
            }
        }
        public bool Found()
        {
            return mySurface != null;
        }
        public double ToleranceReached()
        {
            return myTolReached;
        }



    }
}
