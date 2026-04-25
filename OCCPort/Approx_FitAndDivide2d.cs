using System;

namespace OCCPort
{
    public class Approx_FitAndDivide2d
    {

        //AppParCurves_SequenceOfMultiCurve myMultiCurves;
        TColStd_SequenceOfReal myfirstparam;
        TColStd_SequenceOfReal mylastparam;
        //AppParCurves_MultiCurve TheMultiCurve;
        bool alldone;
        bool tolreached;
        TColStd_SequenceOfReal Tolers3d;
        TColStd_SequenceOfReal Tolers2d;
        int mydegremin;
        int mydegremax;
        double mytol3d;
        double mytol2d;
        double currenttol3d;
        double currenttol2d;
        bool mycut;
        AppParCurves_Constraint myfirstC;
        AppParCurves_Constraint mylastC;
        int myMaxSegments;
        bool myInvOrder;
        bool myHangChecking;

        //! The MultiLine <Line> will be approximated until tolerances
        //! will be reached.
        //! The approximation will be done from degreemin to degreemax
        //! with a cutting if the corresponding boolean is True.
        public Approx_FitAndDivide2d(AppCont_Function Line, int degreemin = 3, int degreemax = 8,

            double Tolerance3d = 1.0e-5, double Tolerance2d = 1.0e-5, bool cutting = false,
            AppParCurves_Constraint FirstC = AppParCurves_Constraint.AppParCurves_TangencyPoint,
            AppParCurves_Constraint LastC = AppParCurves_Constraint.AppParCurves_TangencyPoint)
        {

        }

        //! Initializes the fields of the algorithm.
        public Approx_FitAndDivide2d(int degreemin = 3, int degreemax = 8, double Tolerance3d = 1.0e-05,
            double Tolerance2d = 1.0e-05, bool cutting = false, AppParCurves_Constraint FirstC = AppParCurves_Constraint.AppParCurves_TangencyPoint,
            AppParCurves_Constraint LastC = AppParCurves_Constraint.AppParCurves_TangencyPoint)
        {

        }


        double myTolerance;
        Geom2d_BSplineCurve myBSpline;
        Geom2d_BezierCurve myBezier;
        int myDegMin;
        int myDegMax;

        AppParCurves_Constraint myBndPnt;

        //! Changes the max number of segments, which is allowed for cutting.
        public void SetMaxSegments(int theMaxSegments)
        {
            myMaxSegments = theMaxSegments;

        }

        internal void SetHangChecking(bool theHangChecking)
        {
            myHangChecking = theHangChecking;

        }

        AppParCurves_SequenceOfMultiCurve myMultiCurves;

        //=======================================================================
        //function : Compute
        //purpose  : is internally used by the algorithms.
        //=======================================================================

        public bool Compute(AppCont_Function Line,
  double Ufirst,
  double Ulast,
  ref double TheTol3d,
  ref double TheTol2d)
        {


            int NbPointsMax = 24;
            double aMinRatio = 0.05;
            int aMaxDeg = 8;
            //
            int deg, NbPoints;
            bool mydone;
            double Fv;

            AppParCurves_MultiCurve aPrevCurve;
            double aPrevTol3d = Standard_Real. RealLast(), aPrevTol2d = Standard_Real.RealLast();
            bool aPrevIsOk = false;
            bool anInvOrder = myInvOrder;
            if (anInvOrder && mydegremax > aMaxDeg)
            {
                if ((Ulast - Ufirst) / (Line.LastParameter() - Line.FirstParameter()) < aMinRatio)
                {
                    anInvOrder = false;
                }
            }
            if (anInvOrder)
            {
                //for (deg = mydegremax; deg >= mydegremin; deg--)
                //{
                //    NbPoints = Math.Min(2 * deg + 1, NbPointsMax);
                //    AppCont_LeastSquare LSquare=new AppCont_LeastSquare (Line, Ufirst, Ulast, myfirstC, mylastC, deg, NbPoints);
                //    mydone = LSquare.IsDone();
                //    if (mydone)
                //    {
                //        LSquare.Error(Fv, TheTol3d, TheTol2d);
                //        if (TheTol3d <= mytol3d && TheTol2d <= mytol2d)
                //        {
                //            if (deg == mydegremin)
                //            {
                //                // Stockage de la multicurve approximee.
                //                tolreached = true;
                //                myMultiCurves.Append(LSquare.Value());
                //                myfirstparam.Append(Ufirst);
                //                mylastparam.Append(Ulast);
                //                Tolers3d.Append(TheTol3d);
                //                Tolers2d.Append(TheTol2d);
                //                return true;
                //            }
                //            aPrevTol3d = TheTol3d;
                //            aPrevTol2d = TheTol2d;
                //            aPrevCurve = LSquare.Value();
                //            aPrevIsOk = true;
                //            continue;
                //        }
                //        else if (aPrevIsOk)
                //        {
                //            // Stockage de la multicurve approximee.
                //            tolreached = true;
                //            TheTol3d = aPrevTol3d;
                //            TheTol2d = aPrevTol2d;
                //            myMultiCurves.Append(aPrevCurve);
                //            myfirstparam.Append(Ufirst);
                //            mylastparam.Append(Ulast);
                //            Tolers3d.Append(aPrevTol3d);
                //            Tolers2d.Append(aPrevTol2d);
                //            return Standard_True;
                //        }
                //    }
                //    else if (aPrevIsOk)
                //    {
                //        // Stockage de la multicurve approximee.
                //        tolreached = Standard_True;
                //        TheTol3d = aPrevTol3d;
                //        TheTol2d = aPrevTol2d;
                //        myMultiCurves.Append(aPrevCurve);
                //        myfirstparam.Append(Ufirst);
                //        mylastparam.Append(Ulast);
                //        Tolers3d.Append(aPrevTol3d);
                //        Tolers2d.Append(aPrevTol2d);
                //        return Standard_True;
                //    }
                //    if (!aPrevIsOk && deg == mydegremax)
                //    {
                //        TheMultiCurve = LSquare.Value();
                //        currenttol3d = TheTol3d;
                //        currenttol2d = TheTol2d;
                //        aPrevTol3d = TheTol3d;
                //        aPrevTol2d = TheTol2d;
                //        aPrevCurve = TheMultiCurve;
                //        break;
                //    }
                //}
            }
            else
            {
                //for (deg = mydegremin; deg <= mydegremax; deg++)
                //{
                //    NbPoints = Min(2 * deg + 1, NbPointsMax);
                //    AppCont_LeastSquare LSquare(Line, Ufirst, Ulast, myfirstC, mylastC, deg, NbPoints);
                //    mydone = LSquare.IsDone();
                //    if (mydone)
                //    {
                //        LSquare.Error(Fv, TheTol3d, TheTol2d);
                //        if (TheTol3d <= mytol3d && TheTol2d <= mytol2d)
                //        {
                //            // Stockage de la multicurve approximee.
                //            tolreached = Standard_True;
                //            myMultiCurves.Append(LSquare.Value());
                //            myfirstparam.Append(Ufirst);
                //            mylastparam.Append(Ulast);
                //            Tolers3d.Append(TheTol3d);
                //            Tolers2d.Append(TheTol2d);
                //            return Standard_True;
                //        }
                //    }
                //    if (deg == mydegremax)
                //    {
                //        TheMultiCurve = LSquare.Value();
                //        currenttol3d = TheTol3d;
                //        currenttol2d = TheTol2d;
                //    }
                //}
            }
            return false;
        }
        AppParCurves_MultiCurve TheMultiCurve;

        //! runs the algorithm after having initialized the fields.
        internal void Perform(AppCont_Function Line)
        {
            double UFirst, ULast;
            bool Finish = false,
              begin = true, Ok = false;
            double thetol3d = Precision.Confusion(), thetol2d = Precision.Confusion();
            UFirst = Line.FirstParameter();
            ULast = Line.LastParameter();
            double TolU = 0.0;
            if (myHangChecking)
            {
                TolU = Math.Max((ULast - UFirst) * 1e-03, Precision.Confusion());
            }
            else
            {
                TolU = Math.Max((ULast - UFirst) * 1e-05, Precision.PApproximation());
            }
            double myfirstU = UFirst;
            double mylastU = ULast;
            int aMaxSegments = 0;
            int aMaxSegments1 = myMaxSegments - 1;
            int aNbCut = 0, aNbImp = 0, aNbComp = 10;

            if (!mycut)
            {
                alldone = Compute(Line, UFirst, ULast, ref thetol3d,ref  thetol2d);
                if (!alldone)
                {
                    tolreached = false;
                    //myfirstparam.Append(UFirst);
                    //mylastparam.Append(ULast);
                    //myMultiCurves.Append(TheMultiCurve);
                    //Tolers3d.Append(currenttol3d);
                    //Tolers2d.Append(currenttol2d);
                }
            }
            else
            {

                // previous decision to be taken if we get worse with next cut (eap)
                AppParCurves_MultiCurve KeptMultiCurve = new AppParCurves_MultiCurve();
                double KeptUfirst = 0.0,
                    KeptUlast = 0.0, KeptT3d = Standard_Real.RealLast(), KeptT2d = 0.0;

                while (!Finish)
                {

                    // Gestion du decoupage de la multiline pour approximer:
                    if (!begin)
                    {
                        if (Ok)
                        {
                            // Calcul de la partie a approximer.
                            myfirstU = mylastU;
                            mylastU = ULast;
                            aNbCut = 0;
                            aNbImp = 0;
                            if (Math.Abs(ULast - myfirstU) <= Standard_Real.RealEpsilon()
                                || aMaxSegments >= myMaxSegments)
                            {
                                Finish = true;
                                alldone = true;
                                return;
                            }
                            KeptT3d = Standard_Real. RealLast(); KeptT2d = 0;
                            KeptUfirst = myfirstU;
                            KeptUlast = mylastU;
                        }
                        else
                        {
                            // keep best decision
                            if ((thetol3d + thetol2d) < (KeptT3d + KeptT2d))
                            {
                                KeptMultiCurve = TheMultiCurve;
                                KeptUfirst = myfirstU;
                                KeptUlast = mylastU;
                                KeptT3d = thetol3d;
                                KeptT2d = thetol2d;
                                aNbImp++;
                            }

                            // cut an interval
                            mylastU = (myfirstU + mylastU) / 2;
                            aNbCut++;
                        }
                    }

                    // Calcul des parametres sur ce nouvel intervalle.
                    Ok = Compute(Line, myfirstU, mylastU, ref thetol3d, ref thetol2d);
                    if (Ok)
                    {
                        aMaxSegments++;
                    }

                    //cout << myfirstU << " - " << mylastU << "  tol : " << thetol3d << " " << thetol2d << endl;
                    bool aStopCutting = false;
                    if (myHangChecking && aNbCut >= aNbComp)
                    {
                        if (aNbCut > aNbImp + 1)
                        {
                            aStopCutting = true;
                        }
                        aNbCut = 0;
                        aNbImp = 0;
                    }
                    // is new decision better?
                    if (!Ok && (Math.Abs(myfirstU - mylastU) <= TolU || aMaxSegments >= aMaxSegments1 || aStopCutting))
                    {
                        Ok = true; // stop interval cutting, approx the rest part

                        if ((thetol3d + thetol2d) < (KeptT3d + KeptT2d))
                        {
                            KeptMultiCurve = TheMultiCurve;
                            KeptUfirst = myfirstU;
                            KeptUlast = mylastU;
                            KeptT3d = thetol3d;
                            KeptT2d = thetol2d;
                        }

                        mylastU = KeptUlast;

                        tolreached = false; // helas
                        myMultiCurves.Append(KeptMultiCurve);
                        aMaxSegments++;
                        Tolers3d.Append(KeptT3d);
                        Tolers2d.Append(KeptT2d);
                        myfirstparam.Append(KeptUfirst);
                        mylastparam.Append(KeptUlast);
                    }

                    begin = false;
                } // while (!Finish)
            }
        }
    }
    }