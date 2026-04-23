using System;
using System.Drawing;

namespace OCCPort
{
    //! Class  used  to project  a 3d curve   on a plane.  The
    //! result will be a 3d curve.
    //!
    //! You  can ask   the projected curve  to  have  the same
    //! parametrization as the original curve.
    //!
    //! The projection can be done  along every direction  not
    //! parallel to the plane.
    public class ProjLib_ProjectOnPlane : Adaptor3d_Curve
    {
        public ProjLib_ProjectOnPlane(gp_Ax3 Pl,
     gp_Dir D)
        {
            myPlane = (Pl);
            myDirection = (D);
            myKeepParam = false;
            myFirstPar = (0.0);
            myLastPar = (0.0);
            myTolerance = (0.0);
            myType = GeomAbs_CurveType.GeomAbs_OtherCurve;
            myIsApprox = false;

            //  if ( Abs(D * Pl.Direction()) < Precision::Confusion()) {
            //    throw Standard_ConstructionError
            //      ("ProjLib_ProjectOnPlane:  The Direction and the Plane are parallel");
            //  }
        }
        //=======================================================================
        //function : Project
        //purpose  : Returns the projection of a point <Point> on a plane 
        //           <ThePlane>  along a direction <TheDir>.
        //=======================================================================

        public static gp_Pnt ProjectPnt(gp_Ax3 ThePlane,
  gp_Dir TheDir,
  gp_Pnt Point)
        {
            gp_Vec PO = new gp_Vec(Point, ThePlane.Location());

            double Alpha = PO * new gp_Vec(ThePlane.Direction());
            Alpha /= TheDir * ThePlane.Direction();

            gp_Pnt P = new gp_Pnt();
            P.SetXYZ(Point.XYZ() + TheDir.XYZ().Multiplied(Alpha));

            return P;
        }

        public void Load(Adaptor3d_Curve C,
  double Tolerance,
  bool KeepParametrization)

        {
            myCurve = C;
            myType = GeomAbs_CurveType.GeomAbs_OtherCurve;
            myIsApprox = false;
            myTolerance = Tolerance;

            Geom_BSplineCurve ApproxCurve = null;
            GeomAdaptor_Curve aGAHCurve;

            Geom_Line GeomLinePtr;
            /*Handle(Geom_Circle)    GeomCirclePtr;
            Handle(Geom_Ellipse)   GeomEllipsePtr;
            Handle(Geom_Hyperbola) GeomHyperbolaPtr;
            Handle(Geom_Parabola) GeomParabolaPtr;*/

            gp_Lin aLine;
            //gp_Elips Elips;
            //  gp_Hypr  Hypr ;

            int num_knots;
            GeomAbs_CurveType Type = C.GetType();

            gp_Ax2 Axis;
            double R1 = 0.0, R2 = 0.0;

            myKeepParam = KeepParametrization;

            switch (Type)
            {
                case GeomAbs_CurveType.GeomAbs_Line:
                    {
                        //     P(u) = O + u * Xc
                        // ==> Q(u) = f(P(u)) 
                        //          = f(O) + u * f(Xc)

                        gp_Lin L = myCurve.Line();
                        gp_Vec Xc = ProjectVec(myPlane, myDirection, new gp_Vec(L.Direction()));

                        if (Xc.Magnitude() < Precision.Confusion())
                        { // line orthog au plan
                            myType = GeomAbs_CurveType.GeomAbs_BSplineCurve;
                            gp_Pnt P = ProjectPnt(myPlane, myDirection, L.Location());
                            TColStd_Array1OfInteger Mults=new TColStd_Array1OfInteger (1, 2); 
                            Mults.Init(2);
                            TColgp_Array1OfPnt Poles = new TColgp_Array1OfPnt(1, 2);
                            Poles.Init(P);
                            TColStd_Array1OfReal Knots = new TColStd_Array1OfReal(1, 2);
                            Knots[1] = (float)myCurve.FirstParameter();
                            Knots[2] = (float)myCurve.LastParameter();
                            //Geom_BSplineCurve BSP =
                            //new Geom_BSplineCurve(Poles, Knots, Mults, 1);

                            ////  Modified by Sergey KHROMOV - Tue Jan 29 16:57:29 2002 Begin
                            //GeomAdaptor_Curve aGACurve = new GeomAdaptor_Curve(BSP);
                            //myResult = new GeomAdaptor_Curve(aGACurve);
                            ////  Modified by Sergey KHROMOV - Tue Jan 29 16:57:30 2002 End
                        }
                        else if (Math.Abs(Xc.Magnitude() - 1.0) < Precision.Confusion())
                        {
                            myType = GeomAbs_CurveType.GeomAbs_Line;
                            gp_Pnt P = ProjectPnt(myPlane, myDirection, L.Location());
                            myFirstPar = myCurve.FirstParameter();
                            myLastPar = myCurve.LastParameter();
                            aLine = new gp_Lin(P, new gp_Dir(Xc));
                            GeomLinePtr = new Geom_Line(aLine);

                            //  Modified by Sergey KHROMOV - Tue Jan 29 16:57:29 2002 Begin
                            GeomAdaptor_Curve aGACurve = new GeomAdaptor_Curve(GeomLinePtr,
                              myCurve.FirstParameter(),
                              myCurve.LastParameter());
                            //myResult = new GeomAdaptor_Curve(aGACurve);
                            myResult = aGACurve;
                            //  Modified by Sergey KHROMOV - Tue Jan 29 16:57:30 2002 End
                        }
                        else
                        {
                            myType = GeomAbs_CurveType.GeomAbs_Line;
                            gp_Pnt P = ProjectPnt(myPlane, myDirection, L.Location());
                            aLine = new gp_Lin(P, new gp_Dir(Xc));
                            double Udeb, Ufin;

                            // eval the first and last parameters of the projected curve
                            Udeb = myCurve.FirstParameter();
                            Ufin = myCurve.LastParameter();
                            gp_Pnt P1 = ProjectPnt(myPlane, myDirection,
                              myCurve.Value(Udeb));
                            gp_Pnt P2 = ProjectPnt(myPlane, myDirection,
                              myCurve.Value(Ufin));
                            myFirstPar = new gp_Vec(aLine.Direction()).Dot(new gp_Vec(P, P1));
                            myLastPar = new gp_Vec(aLine.Direction()).Dot(new gp_Vec(P, P2));
                            GeomLinePtr = new Geom_Line(aLine);
                            if (!myKeepParam)
                            {
                                //  Modified by Sergey KHROMOV - Tue Jan 29 16:57:29 2002 Begin
                                GeomAdaptor_Curve aGACurve=new GeomAdaptor_Curve (GeomLinePtr,
                                  myFirstPar,
                                  myLastPar);
                                //myResult = new GeomAdaptor_Curve(aGACurve);
                                myResult = aGACurve;
                                //  Modified by Sergey KHROMOV - Tue Jan 29 16:57:30 2002 End
                            }
                            else
                            {
                                myType = GeomAbs_CurveType.GeomAbs_BSplineCurve;
                                //
                                // make a linear BSpline of degree 1 between the end points of
                                // the projected line 
                                //
                                //Geom_TrimmedCurve NewTrimCurvePtr =
                                //  new Geom_TrimmedCurve(GeomLinePtr,
                                //    myFirstPar,
                                //    myLastPar);

                                //Geom_BSplineCurve NewCurvePtr =
                                //  GeomConvert::CurveToBSplineCurve(NewTrimCurvePtr);
                                //num_knots = NewCurvePtr->NbKnots();
                                //TColStd_Array1OfReal BsplineKnots(1, num_knots);
                                //NewCurvePtr.Knots(BsplineKnots);

                                //BSplCLib::Reparametrize(myCurve.FirstParameter(),
                                //  myCurve.LastParameter(),
                                //  BsplineKnots);

                                //NewCurvePtr.SetKnots(BsplineKnots);
                                ////  Modified by Sergey KHROMOV - Tue Jan 29 16:57:29 2002 Begin
                                //GeomAdaptor_Curve aGACurve = new GeomAdaptor_Curve(NewCurvePtr);
                                //myResult = new GeomAdaptor_Curve(aGACurve);
                                ////  Modified by Sergey KHROMOV - Tue Jan 29 16:57:30 2002 End
                            }
                        }
                        break;
                    }
                default:
                    {
                        myKeepParam = true;
                        myIsApprox = true;
                        myType = GeomAbs_CurveType.GeomAbs_BSplineCurve;
                        //PerformApprox(myCurve, myPlane, myDirection, ApproxCurve);
                        //  Modified by Sergey KHROMOV - Tue Jan 29 16:57:29 2002 Begin
                        GeomAdaptor_Curve aGACurve = new GeomAdaptor_Curve(ApproxCurve);
                        //   myResult = new GeomAdaptor_Curve(aGACurve);
                        //  Modified by Sergey KHROMOV - Tue Jan 29 16:57:30 2002 End
                    }
                    break;
            }
        }



        //=======================================================================
        //function : Project
        //purpose  : Returns the projection of a Vector <Vec> on a plane 
        //           <ThePlane> along a direction <TheDir>.
        //=======================================================================

        public static gp_Vec ProjectVec(gp_Ax3 ThePlane,
  gp_Dir TheDir,
  gp_Vec Vec)
        {
            gp_Vec D = Vec;//todo replace with OpenTK Vector3d
            gp_Vec Z = ThePlane.Direction().to_gp_Vec();

            D -= ((Vec * Z) / (TheDir * Z.To_gp_Dir())) * TheDir;

            return D;
        }

        Adaptor3d_Curve myCurve;
        gp_Ax3 myPlane;
        gp_Dir myDirection;
        bool myKeepParam;
        double myFirstPar;
        double myLastPar;
        double myTolerance;
        GeomAbs_CurveType myType;
        GeomAdaptor_Curve myResult;
        bool myIsApprox;

    }
}