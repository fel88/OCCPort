using OpenTK.Audio.OpenAL;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace OCCPort
{
    internal class Extrema_ELPCOfLocateExtPC
    {
        internal void Initialize(Adaptor3d_Curve C, double Uinf, double Usup, double TolF)
        {
            myC = C;
            myintuinf = myuinf = Uinf;
            myintusup = myusup = Usup;
            mytolf = TolF;
            mytolu = Extrema_CurveTool.Resolution((Adaptor3d_Curve)myC, Precision.Confusion());
            type = Extrema_CurveTool._GetType(C);
            mydone = false;
            mydist1 = Standard_Real.RealLast();
            mydist2 = Standard_Real.RealLast();
            mysample = 17;
        }

        internal bool IsDone()
        {
            return mydone;

        }

        internal int NbExt()
        {
            if (!IsDone()) throw new StdFail_NotDone();
            return mySqDist.Length();
        }


        object myC;
        gp_Pnt Pf;
        gp_Pnt Pl;
        Extrema_ExtPElC myExtPElC = new Extrema_ExtPElC();
        Extrema_SequenceOfPOnCurv mypoint = new Extrema_SequenceOfPOnCurv();
        bool mydone;
        double mydist1;
        double mydist2;
        Extrema_EPCOfELPCOfLocateExtPC myExtPC;
        double mytolu;
        double mytolf;
        int mysample;
        double myintuinf;
        double myintusup;
        double myuinf;
        double myusup;
        GeomAbs_CurveType type;
        TColStd_SequenceOfBoolean myismin = new TColStd_SequenceOfBoolean();
        TColStd_SequenceOfReal mySqDist = new TColStd_SequenceOfReal();

        internal void Perform(gp_Pnt P)
        {
            mySqDist.Clear();
            mypoint.Clear();
            myismin.Clear();
            int i, NbExt, n;
            double U;
            mysample = 17;
            double t3d = Precision.Confusion();
            if (Precision.IsInfinite(myuinf)) mydist1 = Standard_Real.RealLast();
            else
            {
                Pf = Extrema_CurveTool.Value((Adaptor3d_Curve)myC, myuinf);
                mydist1 = P.SquareDistance(Pf);
            }

            if (Precision.IsInfinite(myusup)) mydist2 = Standard_Real.RealLast();
            else
            {
                Pl = Extrema_CurveTool.Value((Adaptor3d_Curve)myC, myusup);
                mydist2 = P.SquareDistance(Pl);
            }

            Adaptor3d_Curve aCurve = (Adaptor3d_Curve)myC;

            switch (type)
            {
                //case GeomAbs_Circle:
                //    {
                //        myExtPElC.Perform(P, TheCurveTool::Circle(aCurve), t3d, myuinf, myusup);
                //    }
                //    break;
                //case GeomAbs_Ellipse:
                //    {
                //        myExtPElC.Perform(P, TheCurveTool::Ellipse(aCurve), t3d, myuinf, myusup);
                //    }
                //    break;
                //case GeomAbs_Parabola:
                //    {
                //        myExtPElC.Perform(P, TheCurveTool::Parabola(aCurve), t3d, myuinf, myusup);
                //    }
                //    break;
                //case GeomAbs_Hyperbola:
                //    {
                //        myExtPElC.Perform(P, TheCurveTool::Hyperbola(aCurve), t3d, myuinf, myusup);
                //    }
                //    break;
                case GeomAbs_CurveType. GeomAbs_Line:
                    {
                        myExtPElC.Perform(P, Extrema_CurveTool.Line(aCurve), t3d, myuinf, myusup);
                    }
                    break;
                //case GeomAbs_BezierCurve:
                //    {
                //        myintuinf = myuinf;
                //        myintusup = myusup;
                //        mysample = (TheCurveTool::Bezier(aCurve))->NbPoles() * 2;
                //        myExtPC.Initialize(aCurve);
                //        IntervalPerform(P);
                //        return;
                //    }
                //case GeomAbs_BSplineCurve:
                //    {
                //        const Standard_Integer
                //          aFirstIdx = TheCurveTool::BSpline(aCurve)->FirstUKnotIndex(),
                //          aLastIdx = TheCurveTool::BSpline(aCurve)->LastUKnotIndex();
                //        // const reference can not be used due to implementation of BRep_Adaptor.
                //        TColStd_Array1OfReal aKnots(aFirstIdx, aLastIdx);
                //        TheCurveTool::BSpline(aCurve)->Knots(aKnots);

                //        // Workaround to work with:
                //        // blend, where knots may be moved from parameter space.
                //        Standard_Real aPeriodJump = 0.0;
                //        // Avoid problem with too close knots.
                //        const Standard_Real aTolCoeff = (myusup - myuinf) * Precision::PConfusion();
                //        if (TheCurveTool::IsPeriodic(aCurve))
                //        {
                //            Standard_Integer aPeriodShift =
                //              Standard_Integer((myuinf - aKnots(aFirstIdx)) / TheCurveTool::Period(aCurve));

                //            if (myuinf < aKnots(aFirstIdx) - aTolCoeff)
                //                aPeriodShift--;

                //            aPeriodJump = TheCurveTool::Period(aCurve) * aPeriodShift;
                //        }

                //        Standard_Integer anIdx;

                //        // Find first and last used knot.
                //        Standard_Integer aFirstUsedKnot = aFirstIdx,
                //                          aLastUsedKnot = aLastIdx;
                //        for (anIdx = aFirstIdx; anIdx <= aLastIdx; anIdx++)
                //        {
                //            Standard_Real aKnot = aKnots(anIdx) + aPeriodJump;
                //            if (myuinf >= aKnot - aTolCoeff)
                //                aFirstUsedKnot = anIdx;
                //            else
                //                break;

                //        }
                //        for (anIdx = aLastIdx; anIdx >= aFirstIdx; anIdx--)
                //        {
                //            Standard_Real aKnot = aKnots(anIdx) + aPeriodJump;
                //            if (myusup <= aKnot + aTolCoeff)
                //                aLastUsedKnot = anIdx;
                //            else
                //                break;
                //        }

                //        if (aFirstUsedKnot == aLastUsedKnot)
                //        {
                //            // Degenerated case:
                //            // Some bounds lies out of curve param space.
                //            // In this case build one interval with [myuinf, myusup].
                //            // Parameters of these indexes will be redefined.
                //            aFirstUsedKnot = aFirstIdx;
                //            aLastUsedKnot = aFirstIdx + 1;
                //        }

                //        mysample = (TheCurveTool::BSpline(aCurve))->Degree() + 1;

                //        if (mysample == 2)
                //        {
                //            //BSpline of first degree, direct searching extrema for each knot interval
                //            ThePoint aPmin;
                //            Standard_Real tmin = 0., distmin = RealLast();
                //            Standard_Real aMin1 = 0., aMin2 = 0.;
                //            myExtPC.Initialize(aCurve);
                //            for (anIdx = aFirstUsedKnot; anIdx < aLastUsedKnot; anIdx++)
                //            {
                //                Standard_Real aF = aKnots(anIdx) + aPeriodJump,
                //                  aL = aKnots(anIdx + 1) + aPeriodJump;

                //                if (anIdx == aFirstUsedKnot)
                //                {
                //                    aF = myuinf;
                //                }
                //                else
                //                if (anIdx == aLastUsedKnot - 1)
                //                {
                //                    aL = myusup;
                //                }


                //                ThePoint aP1, aP2;
                //                TheCurveTool::D0(aCurve, aF, aP1);
                //                TheCurveTool::D0(aCurve, aL, aP2);
                //                TheVector aBase1(P, aP1), aBase2(P, aP2);
                //                TheVector aV(aP2, aP1);
                //                Standard_Real aVal1 = aV.Dot(aBase1); // Derivative of (C(u) - P)^2
                //                Standard_Real aVal2 = aV.Dot(aBase2); // Derivative of (C(u) - P)^2
                //                if (anIdx == aFirstUsedKnot)
                //                {
                //                    aMin1 = P.SquareDistance(aP1);
                //                }
                //                else
                //                {
                //                    aMin1 = aMin2;
                //                    if (distmin > aMin1)
                //                    {
                //                        distmin = aMin1;
                //                        tmin = aF;
                //                        aPmin = aP1;
                //                    }
                //                }
                //                aMin2 = P.SquareDistance(aP2);
                //                Standard_Real aMinSqDist = Min(aMin1, aMin2);
                //                Standard_Real aMinDer = Min(Abs(aVal1), Abs(aVal2));
                //                if (!(Precision::IsInfinite(aVal1) || Precision::IsInfinite(aVal2)))
                //                {
                //                    // Derivatives have opposite signs - min or max inside of interval (sufficient condition).
                //                    if (aVal1 * aVal2 <= 0.0 ||
                //                        aMinSqDist < 100. * Precision::SquareConfusion() ||
                //                        2.* aMinDer < Precision::Confusion())
                //                    {
                //                        myintuinf = aF;
                //                        myintusup = aL;
                //                        IntervalPerform(P);
                //                    }
                //                }
                //            }
                //            if (!Precision::IsInfinite(distmin))
                //            {
                //                Standard_Boolean isToAdd = Standard_True;
                //                NbExt = mypoint.Length();
                //                for (i = 1; i <= NbExt && isToAdd; i++)
                //                {
                //                    Standard_Real t = mypoint.Value(i).Parameter();
                //                    isToAdd = (distmin < mySqDist(i)) && (Abs(t - tmin) > mytolu);
                //                }
                //                if (isToAdd)
                //                {
                //                    ThePOnC PC(tmin, aPmin);
                //                    mySqDist.Append(distmin);
                //                    myismin.Append(Standard_True);
                //                    mypoint.Append(PC);
                //                }
                //            }
                //        }
                //        else
                //        {

                //            // Fill sample points.
                //            Standard_Integer aValIdx = 1;
                //            NCollection_Array1<Standard_Real> aVal(1, (mysample) * (aLastUsedKnot - aFirstUsedKnot) + 1);
                //            NCollection_Array1<Standard_Real> aParam(1, (mysample) * (aLastUsedKnot - aFirstUsedKnot) + 1);
                //            for (anIdx = aFirstUsedKnot; anIdx < aLastUsedKnot; anIdx++)
                //            {
                //                Standard_Real aF = aKnots(anIdx) + aPeriodJump,
                //                  aL = aKnots(anIdx + 1) + aPeriodJump;

                //                if (anIdx == aFirstUsedKnot)
                //                    aF = myuinf;
                //                if (anIdx == aLastUsedKnot - 1)
                //                    aL = myusup;

                //                Standard_Real aStep = (aL - aF) / mysample;
                //                for (Standard_Integer aPntIdx = 0; aPntIdx < mysample; aPntIdx++)
                //                {
                //                    Standard_Real aCurrentParam = aF + aStep * aPntIdx;
                //                    aVal(aValIdx) = TheCurveTool::Value(aCurve, aCurrentParam).SquareDistance(P);
                //                    aParam(aValIdx) = aCurrentParam;
                //                    aValIdx++;
                //                }
                //            }
                //            // Fill last point.
                //            aVal(aValIdx) = TheCurveTool::Value(aCurve, myusup).SquareDistance(P);
                //            aParam(aValIdx) = myusup;

                //            myExtPC.Initialize(aCurve);

                //            // Find extremas.
                //            for (anIdx = aVal.Lower() + 1; anIdx < aVal.Upper(); anIdx++)
                //            {
                //                if (aVal(anIdx) <= Precision::SquareConfusion())
                //                {
                //                    mySqDist.Append(aVal(anIdx));
                //                    myismin.Append(Standard_True);
                //                    mypoint.Append(ThePOnC(aParam(anIdx), TheCurveTool::Value(aCurve, aParam(anIdx))));
                //                }
                //                if ((aVal(anIdx) >= aVal(anIdx + 1) &&
                //                  aVal(anIdx) >= aVal(anIdx - 1)) ||
                //                  (aVal(anIdx) <= aVal(anIdx + 1) &&
                //                  aVal(anIdx) <= aVal(anIdx - 1)))
                //                {
                //                    myintuinf = aParam(anIdx - 1);
                //                    myintusup = aParam(anIdx + 1);

                //                    IntervalPerform(P);
                //                }
                //            }

                //            // Solve on the first and last intervals.
                //            if (mydist1 > Precision::SquareConfusion() && !Precision::IsPositiveInfinite(mydist1))
                //            {
                //                ThePoint aP1, aP2;
                //                TheVector aV1, aV2;
                //                TheCurveTool::D1(aCurve, aParam.Value(aParam.Lower()), aP1, aV1);
                //                TheCurveTool::D1(aCurve, aParam.Value(aParam.Lower() + 1), aP2, aV2);
                //                TheVector aBase1(P, aP1), aBase2(P, aP2);
                //                Standard_Real aVal1 = aV1.Dot(aBase1); // Derivative of (C(u) - P)^2
                //                Standard_Real aVal2 = aV2.Dot(aBase2); // Derivative of (C(u) - P)^2
                //                if (!(Precision::IsInfinite(aVal1) || Precision::IsInfinite(aVal2)))
                //                {
                //                    // Derivatives have opposite signs - min or max inside of interval (sufficient condition).
                //                    // Necessary condition - when point lies on curve.
                //                    // Necessary condition - when derivative of point is too small.
                //                    if (aVal1 * aVal2 <= 0.0 ||
                //                      aBase1.Dot(aBase2) <= 0.0 ||
                //                      2.0 * Abs(aVal1) < Precision::Confusion())
                //                    {
                //                        myintuinf = aParam(aVal.Lower());
                //                        myintusup = aParam(aVal.Lower() + 1);
                //                        IntervalPerform(P);
                //                    }
                //                }
                //            }

                //            if (mydist2 > Precision::SquareConfusion() && !Precision::IsPositiveInfinite(mydist2))
                //            {
                //                ThePoint aP1, aP2;
                //                TheVector aV1, aV2;
                //                TheCurveTool::D1(aCurve, aParam.Value(aParam.Upper() - 1), aP1, aV1);
                //                TheCurveTool::D1(aCurve, aParam.Value(aParam.Upper()), aP2, aV2);
                //                TheVector aBase1(P, aP1), aBase2(P, aP2);
                //                Standard_Real aVal1 = aV1.Dot(aBase1); // Derivative of (C(u) - P)^2
                //                Standard_Real aVal2 = aV2.Dot(aBase2); // Derivative of (C(u) - P)^2

                //                if (!(Precision::IsInfinite(aVal1) || Precision::IsInfinite(aVal2)))
                //                {
                //                    // Derivatives have opposite signs - min or max inside of interval (sufficient condition).
                //                    // Necessary condition - when point lies on curve.
                //                    // Necessary condition - when derivative of point is too small.
                //                    if (aVal1 * aVal2 <= 0.0 ||
                //                      aBase1.Dot(aBase2) <= 0.0 ||
                //                      2.0 * Abs(aVal2) < Precision::Confusion())
                //                    {
                //                        myintuinf = aParam(aVal.Upper() - 1);
                //                        myintusup = aParam(aVal.Upper());
                //                        IntervalPerform(P);
                //                    }
                //                }
                //            }
                //        }
                //        mydone = Standard_True;
                //        break;
                //    }
                //default:
                //    {
                //        const Standard_Integer aMaxSample = 17;
                //        Standard_Boolean IntExtIsDone = Standard_False;
                //        Standard_Boolean IntIsNotValid;
                //        Handle(TColStd_HArray1OfReal) theHInter;
                //        n = TheCurveTool::NbIntervals(aCurve, GeomAbs_C2);
                //        if (n > 1)
                //        {
                //            theHInter = new TColStd_HArray1OfReal(1, n + 1);
                //            TheCurveTool::Intervals(aCurve, theHInter->ChangeArray1(), GeomAbs_C2);
                //        }
                //        else
                //        {
                //            theHInter = TheCurveTool::DeflCurvIntervals(aCurve);
                //            n = theHInter->Length() - 1;
                //        }
                //        mysample = Max(mysample / n, aMaxSample);
                //        Standard_Real maxint = 0.;
                //        for (i = 1; i <= n; ++i)
                //        {
                //            Standard_Real dt = theHInter->Value(i + 1) - theHInter->Value(i);
                //            if (maxint < dt)
                //            {
                //                maxint = dt;
                //            }
                //        }
                //        Standard_Boolean isPeriodic = TheCurveTool::IsPeriodic(aCurve);
                //        TheVector V1;
                //        ThePoint PP;
                //        Standard_Real s1 = 0.0;
                //        Standard_Real s2 = 0.0;
                //        myExtPC.Initialize(aCurve);
                //        for (i = 1; i <= n; i++)
                //        {
                //            myintuinf = theHInter->Value(i);
                //            myintusup = theHInter->Value(i + 1);
                //            mysample = Max(RealToInt(aMaxSample * (myintusup - myintuinf) / maxint), 3);

                //            Standard_Real anInfToCheck = myintuinf;
                //            Standard_Real aSupToCheck = myintusup;

                //            if (isPeriodic)
                //            {
                //                Standard_Real aPeriod = TheCurveTool::Period(aCurve);
                //                anInfToCheck = ElCLib::InPeriod(myintuinf, myuinf, myuinf + aPeriod);
                //                aSupToCheck = myintusup + (anInfToCheck - myintuinf);
                //            }
                //            IntIsNotValid = (myuinf > aSupToCheck) || (myusup < anInfToCheck);

                //            if (IntIsNotValid) continue;

                //            if (myuinf >= anInfToCheck) anInfToCheck = myuinf;
                //            if (myusup <= aSupToCheck) aSupToCheck = myusup;
                //            if ((aSupToCheck - anInfToCheck) <= mytolu) continue;

                //            if (i != 1)
                //            {
                //                TheCurveTool::D1(aCurve, myintuinf, PP, V1);
                //                s1 = (TheVector(P, PP)) * V1;
                //                if (s1 * s2 < 0.0)
                //                {
                //                    mySqDist.Append(PP.SquareDistance(P));
                //                    myismin.Append((s1 < 0.0));
                //                    mypoint.Append(ThePOnC(myintuinf, PP));
                //                }
                //            }
                //            if (i != n)
                //            {
                //                TheCurveTool::D1(aCurve, myintusup, PP, V1);
                //                s2 = (TheVector(P, PP)) * V1;
                //            }

                //            IntervalPerform(P);
                //            IntExtIsDone = IntExtIsDone || mydone;
                //        }

                //        mydone = IntExtIsDone;
                //        break;
                //    }
            }

            // Postprocessing.
            //if (type == GeomAbs_BSplineCurve ||
            //    type == GeomAbs_OffsetCurve ||
            //    type == GeomAbs_OtherCurve)
            //{
            //    // Additional checking if the point is on the first or last point of the curve
            //    // and does not added yet.
            //    if (mydist1 < Precision::SquareConfusion() ||
            //        mydist2 < Precision::SquareConfusion())
            //    {
            //        Standard_Boolean isFirstAdded = Standard_False;
            //        Standard_Boolean isLastAdded = Standard_False;
            //        Standard_Integer aNbPoints = mypoint.Length();
            //        for (i = 1; i <= aNbPoints; i++)
            //        {
            //            U = mypoint.Value(i).Parameter();
            //            if (Abs(U - myuinf) < mytolu)
            //                isFirstAdded = Standard_True;
            //            else if (Abs(myusup - U) < mytolu)
            //                isLastAdded = Standard_True;
            //        }
            //        if (!isFirstAdded && mydist1 < Precision::SquareConfusion())
            //        {
            //            mySqDist.Prepend(mydist1);
            //            myismin.Prepend(Standard_True);
            //            mypoint.Prepend(ThePOnC(myuinf, Pf));
            //        }
            //        if (!isLastAdded && mydist2 < Precision::SquareConfusion())
            //        {
            //            mySqDist.Append(mydist2);
            //            myismin.Append(Standard_True);
            //            mypoint.Append(ThePOnC(myusup, Pl));
            //        }
            //        mydone = Standard_True;
            //    }
            //}
            //else
            {
                // In analytical case
                mydone = myExtPElC.IsDone();
                if (mydone)
                {
                    NbExt = myExtPElC.NbExt();
                    for (i = 1; i <= NbExt; i++)
                    {
                        // Verification de la validite des parametres:
                        Extrema_POnCurv PC = myExtPElC.Point(i);
                        U = PC.Parameter();
                        if (Extrema_CurveTool.IsPeriodic(aCurve))
                        {
                            U = ElCLib.InPeriod(U, myuinf, myuinf + Extrema_CurveTool.Period(aCurve));
                        }
                        if ((U >= myuinf - mytolu) && (U <= myusup + mytolu))
                        {
                            PC.SetValues(U, myExtPElC.Point(i).Value());
                            mySqDist.Append(myExtPElC.SquareDistance(i));
                            myismin.Append(myExtPElC.IsMin(i));
                            mypoint.Append(PC);
                        }
                    }
                }
            }
        }

        internal Extrema_POnCurv Point(int N)
        {
            if ((N < 1) || (N > NbExt())) throw new Standard_OutOfRange();
            return mypoint.Value(N);
        }
    }
}