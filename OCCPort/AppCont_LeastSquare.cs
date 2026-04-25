using System;
using System.Runtime.Intrinsics.X86;

namespace OCCPort
{
    public class AppCont_LeastSquare
    {
        AppParCurves_MultiCurve mySCU;
        math_Matrix myPoints;
        math_Matrix myPoles;
        math_Vector myParam;
        math_Matrix myVB;
        NCollection_Array1<PeriodicityInfo> myPerInfo;
        bool myDone;
        int myDegre;
        int myNbdiscret, myNbP, myNbP2d;
        public AppCont_LeastSquare(AppCont_Function SSP,
                                         double U0,
                                         double U1,
                                         AppParCurves_Constraint FirstCons,
                                         AppParCurves_Constraint LastCons,
                                         int Deg,
                                         int myNbPoints)
        {
            //mySCU = (Deg + 1);
            //myPoints(1, myNbPoints, 1, 3 * SSP.GetNbOf3dPoints() + 2 * SSP.GetNbOf2dPoints()),
            //myPoles(1, Deg + 1, 1, 3 * SSP.GetNbOf3dPoints() + 2 * SSP.GetNbOf2dPoints(), 0.0),
            //myParam(1, myNbPoints),
            //myVB(1, Deg+1, 1, myNbPoints),
            //myPerInfo(1, 3 * SSP.GetNbOf3dPoints() + 2 * SSP.GetNbOf2dPoints() )

            myDone = false;
            myDegre = Deg;
            int i, j, k, c, i2;
            int classe = Deg + 1, cl1 = Deg;
            double U, dU, Coeff, Coeff2;
            double IBij, IBPij;

            int FirstP = 1, LastP = myNbPoints;
            int nbcol = 3 * SSP.GetNbOf3dPoints() + 2 * SSP.GetNbOf2dPoints();
            math_Matrix B = new math_Matrix(1, classe, 1, nbcol, 0.0);
            int bdeb = 1, bfin = classe;
            AppParCurves_Constraint myFirstC = FirstCons, myLastC = LastCons;
            SSP.GetNumberOfPoints(ref myNbP, ref myNbP2d);

            int i2plus1, i2plus2;
            myNbdiscret = myNbPoints;
            NCollection_Array1<gp_Pnt> aTabP = new NCollection_Array1<gp_Pnt>(1, Math.Max(myNbP, 1));
            NCollection_Array1<gp_Pnt2d> aTabP2d = new NCollection_Array1<gp_Pnt2d>(1, Math.Max(myNbP2d, 1));
            NCollection_Array1<gp_Vec> aTabV = new NCollection_Array1<gp_Vec>(1, Math.Max(myNbP, 1));
            NCollection_Array1<gp_Vec2d> aTabV2d = new NCollection_Array1<gp_Vec2d>(1, Math.Max(myNbP2d, 1));

            for (int aDimIdx = 1; aDimIdx <= myNbP * 3 + myNbP2d * 2; aDimIdx++)
            {
                SSP.PeriodInformation(aDimIdx,
                                  out bool t1,
                                  out double t2);
                myPerInfo[aDimIdx] = new PeriodicityInfo() { isPeriodic = t1, myPeriod = t2 };                
            }

            bool Ok;
            if (myFirstC == AppParCurves_Constraint.AppParCurves_TangencyPoint)
            {
                Ok = SSP.D1(U0, aTabV2d, aTabV);
                if (!Ok) myFirstC = AppParCurves_Constraint.AppParCurves_PassPoint;
            }

            if (myLastC == AppParCurves_Constraint.AppParCurves_TangencyPoint)
            {
                Ok = SSP.D1(U1, aTabV2d, aTabV);
                if (!Ok) myLastC = AppParCurves_Constraint.AppParCurves_PassPoint;
            }

            // Compute control points params on which approximation will be built.
            math_Vector GaussP = new math_Vector(1, myNbPoints), GaussW = new math_Vector(1, myNbPoints);
            math.GaussPoints(myNbPoints, ref GaussP);
            math.GaussWeights(myNbPoints, ref GaussW);
            math_Vector TheWeights = new math_Vector(1, myNbPoints), VBParam = new math_Vector(1, myNbPoints);
            dU = 0.5 * (U1 - U0);
            for (i = FirstP; i <= LastP; i++)
            {
                U = 0.5 * (U1 + U0) + dU * GaussP[i];
                if (i <= (myNbPoints + 1) / 2)
                {
                    myParam[LastP - i + 1] = U;
                    VBParam[LastP - i + 1] = 0.5 * (1 + GaussP[i]);
                    TheWeights[LastP - i + 1] = 0.5 * GaussW[i];
                }
                else
                {
                    VBParam[i - (myNbPoints + 1) / 2] = 0.5 * (1 + GaussP[i]);
                    myParam[i - (myNbPoints + 1) / 2] = U;
                    TheWeights[i - (myNbPoints + 1) / 2] = 0.5 * GaussW[i];
                }
            }

            //// Compute control points.
            //for (i = FirstP; i <= LastP; i++)
            //{
            //    U = myParam(i);
            //    SSP.Value(U, aTabP2d, aTabP);

            //    i2 = 1;
            //    for (j = 1; j <= myNbP; j++)
            //    {
            //        (aTabP(j)).Coord(myPoints(i, i2), myPoints(i, i2 + 1), myPoints(i, i2 + 2));
            //        i2 += 3;
            //    }
            //    for (j = 1; j <= myNbP2d; j++)
            //    {
            //        (aTabP2d(j)).Coord(myPoints(i, i2), myPoints(i, i2 + 1));
            //        i2 += 2;
            //    }
            //}

            //// Fix possible "period jump".
            //int aMaxDim = 3 * myNbP + 2 * myNbP2d;
            //for (int aDimIdx = 1; aDimIdx <= aMaxDim; aDimIdx++)
            //{
            //    if (myPerInfo(aDimIdx).isPeriodic &&
            //        Abs(myPoints(1, aDimIdx) - myPoints(2, aDimIdx)) > myPerInfo(aDimIdx).myPeriod / 2.01 &&
            //        Abs(myPoints(2, aDimIdx) - myPoints(3, aDimIdx)) < myPerInfo(aDimIdx).myPeriod / 2.01)
            //    {
            //        double aPeriodMult = (myPoints(1, aDimIdx) < myPoints(2, aDimIdx)) ? 1.0 : -1.0;
            //        double aNewParam = myPoints(1, aDimIdx) + aPeriodMult * myPerInfo(aDimIdx).myPeriod;
            //        myPoints(1, aDimIdx) = aNewParam;
            //    }
            //}
            //for (int aPntIdx = 1; aPntIdx < myNbPoints; aPntIdx++)
            //{
            //    for (int aDimIdx = 1; aDimIdx <= aMaxDim; aDimIdx++)
            //    {
            //        if (myPerInfo(aDimIdx).isPeriodic &&
            //          Abs(myPoints(aPntIdx, aDimIdx) - myPoints(aPntIdx + 1, aDimIdx)) > myPerInfo(aDimIdx).myPeriod / 2.01)
            //        {
            //            Standard_Real aPeriodMult = (myPoints(aPntIdx, aDimIdx) > myPoints(aPntIdx + 1, aDimIdx)) ? 1.0 : -1.0;
            //            Standard_Real aNewParam = myPoints(aPntIdx + 1, aDimIdx) + aPeriodMult * myPerInfo(aDimIdx).myPeriod;
            //            myPoints(aPntIdx + 1, aDimIdx) = aNewParam;
            //        }
            //    }
            //}

            //AppCont_ContMatrices.VBernstein(classe, myNbPoints, ref myVB);

            //// Traitement du second membre:
            //NCollection_Array1<double> tmppoints = new NCollection_Array1<double>(1, nbcol);

            //for (c = 1; c <= classe; c++)
            //{
            //    tmppoints.Init(0.0);
            //    for (i = 1; i <= myNbPoints; i++)
            //    {
            //        Coeff = TheWeights(i) * myVB(c, i);
            //        for (j = 1; j <= nbcol; j++)
            //        {
            //            tmppoints[j] += myPoints[i, j] * Coeff;
            //        }
            //    }
            //    for (k = 1; k <= nbcol; k++)
            //    {
            //        B(c, k) += tmppoints(k);
            //    }
            //}

            //if (myFirstC == AppParCurves_NoConstraint &&
            //    myLastC == AppParCurves_NoConstraint)
            //{

            //    math_Matrix InvM(1, classe, 1, classe);
            //    InvMMatrix(classe, InvM);
            //    // Calcul direct des poles:

            //    for (i = 1; i <= classe; i++)
            //    {
            //        for (j = 1; j <= classe; j++)
            //        {
            //            IBij = InvM(i, j);
            //            for (k = 1; k <= nbcol; k++)
            //            {
            //                myPoles(i, k) += IBij * B(j, k);
            //            }
            //        }
            //    }
            //}


            //else
            //{
            //    math_Matrix M(1, classe, 1, classe);
            //    MMatrix(classe, M);
            //    NCollection_Array1<gp_Pnt2d> aFixP2d(1, Max(myNbP2d, 1));
            //    NCollection_Array1<gp_Pnt> aFixP(1, Max(myNbP, 1));

            //    if (myFirstC == AppParCurves_PassPoint ||
            //        myFirstC == AppParCurves_TangencyPoint)
            //    {
            //        SSP.Value(U0, aTabP2d, aTabP);
            //        FixSingleBorderPoint(SSP, U0, U0, U1, aFixP2d, aFixP);

            //        i2 = 1;
            //        for (k = 1; k <= myNbP; k++)
            //        {
            //            if (aFixP(k).Distance(aTabP(k)) > 0.1)
            //                (aFixP(k)).Coord(myPoles(1, i2), myPoles(1, i2 + 1), myPoles(1, i2 + 2));
            //            else
            //                (aTabP(k)).Coord(myPoles(1, i2), myPoles(1, i2 + 1), myPoles(1, i2 + 2));
            //            i2 += 3;
            //        }
            //        for (k = 1; k <= myNbP2d; k++)
            //        {
            //            if (aFixP2d(k).Distance(aTabP2d(k)) > 0.1)
            //                (aFixP2d(k)).Coord(myPoles(1, i2), myPoles(1, i2 + 1));
            //            else
            //                (aTabP2d(k)).Coord(myPoles(1, i2), myPoles(1, i2 + 1));
            //            i2 += 2;
            //        }

            //        for (Standard_Integer aDimIdx = 1; aDimIdx <= aMaxDim; aDimIdx++)
            //        {
            //            if (myPerInfo(aDimIdx).isPeriodic &&
            //                Abs(myPoles(1, aDimIdx) - myPoints(1, aDimIdx)) > myPerInfo(aDimIdx).myPeriod / 2.01)
            //            {
            //                Standard_Real aMult = myPoles(1, aDimIdx) < myPoints(1, aDimIdx) ? 1.0 : -1.0;
            //                myPoles(1, aDimIdx) += aMult * myPerInfo(aDimIdx).myPeriod;
            //            }
            //        }
            //    }

            //    if (myLastC == AppParCurves_PassPoint ||
            //        myLastC == AppParCurves_TangencyPoint)
            //    {
            //        SSP.Value(U1, aTabP2d, aTabP);
            //        FixSingleBorderPoint(SSP, U1, U0, U1, aFixP2d, aFixP);

            //        i2 = 1;
            //        for (k = 1; k <= myNbP; k++)
            //        {
            //            if (aFixP(k).Distance(aTabP(k)) > 0.1)
            //                (aFixP(k)).Coord(myPoles(classe, i2), myPoles(classe, i2 + 1), myPoles(classe, i2 + 2));
            //            else
            //                (aTabP(k)).Coord(myPoles(classe, i2), myPoles(classe, i2 + 1), myPoles(classe, i2 + 2));
            //            i2 += 3;
            //        }
            //        for (k = 1; k <= myNbP2d; k++)
            //        {
            //            if (aFixP2d(k).Distance(aTabP2d(k)) > 0.1)
            //                (aFixP2d(k)).Coord(myPoles(classe, i2), myPoles(classe, i2 + 1));
            //            else
            //                (aTabP2d(k)).Coord(myPoles(classe, i2), myPoles(classe, i2 + 1));
            //            i2 += 2;
            //        }


            //        for (Standard_Integer aDimIdx = 1; aDimIdx <= 2; aDimIdx++)
            //        {
            //            if (myPerInfo(aDimIdx).isPeriodic &&
            //              Abs(myPoles(classe, aDimIdx) - myPoints(myNbPoints, aDimIdx)) > myPerInfo(aDimIdx).myPeriod / 2.01)
            //            {
            //                Standard_Real aMult = myPoles(classe, aDimIdx) < myPoints(myNbPoints, aDimIdx) ? 1.0 : -1.0;
            //                myPoles(classe, aDimIdx) += aMult * myPerInfo(aDimIdx).myPeriod;
            //            }
            //        }
            //    }

            //    if (myFirstC == AppParCurves_PassPoint)
            //    {
            //        bdeb = 2;
            //        // mise a jour du second membre:
            //        for (i = 1; i <= classe; i++)
            //        {
            //            Coeff = M(i, 1);
            //            for (k = 1; k <= nbcol; k++)
            //            {
            //                B(i, k) -= myPoles(1, k) * Coeff;
            //            }
            //        }
            //    }

            //    if (myLastC == AppParCurves_PassPoint)
            //    {
            //        bfin = cl1;
            //        for (i = 1; i <= classe; i++)
            //        {
            //            Coeff = M(i, classe);
            //            for (k = 1; k <= nbcol; k++)
            //            {
            //                B(i, k) -= myPoles(classe, k) * Coeff;
            //            }
            //        }
            //    }

            //    if (myFirstC == AppParCurves_TangencyPoint)
            //    {
            //        // On fixe le second pole::
            //        bdeb = 3;
            //        SSP.D1(U0, aTabV2d, aTabV);

            //        i2 = 1;
            //        Coeff = (U1 - U0) / myDegre;
            //        for (k = 1; k <= myNbP; k++)
            //        {
            //            i2plus1 = i2 + 1; i2plus2 = i2 + 2;
            //            myPoles(2, i2) = myPoles(1, i2) + aTabV(k).X() * Coeff;
            //            myPoles(2, i2plus1) = myPoles(1, i2plus1) + aTabV(k).Y() * Coeff;
            //            myPoles(2, i2plus2) = myPoles(1, i2plus2) + aTabV(k).Z() * Coeff;
            //            i2 += 3;
            //        }
            //        for (k = 1; k <= myNbP2d; k++)
            //        {
            //            i2plus1 = i2 + 1;
            //            myPoles(2, i2) = myPoles(1, i2) + aTabV2d(k).X() * Coeff;
            //            myPoles(2, i2plus1) = myPoles(1, i2plus1) + aTabV2d(k).Y() * Coeff;
            //            i2 += 2;
            //        }

            //        for (i = 1; i <= classe; i++)
            //        {
            //            Coeff = M(i, 1); Coeff2 = M(i, 2);
            //            for (k = 1; k <= nbcol; k++)
            //            {
            //                B(i, k) -= myPoles(1, k) * Coeff + myPoles(2, k) * Coeff2;
            //            }
            //        }
            //    }

            //    if (myLastC == AppParCurves_TangencyPoint)
            //    {
            //        bfin = classe - 2;
            //        SSP.D1(U1, aTabV2d, aTabV);
            //        i2 = 1;
            //        Coeff = (U1 - U0) / myDegre;
            //        for (k = 1; k <= myNbP; k++)
            //        {
            //            i2plus1 = i2 + 1; i2plus2 = i2 + 2;
            //            myPoles(cl1, i2) = myPoles(classe, i2) - aTabV(k).X() * Coeff;
            //            myPoles(cl1, i2plus1) = myPoles(classe, i2plus1) - aTabV(k).Y() * Coeff;
            //            myPoles(cl1, i2plus2) = myPoles(classe, i2plus2) - aTabV(k).Z() * Coeff;
            //            i2 += 3;
            //        }
            //        for (k = 1; k <= myNbP2d; k++)
            //        {
            //            i2plus1 = i2 + 1;
            //            myPoles(cl1, i2) = myPoles(classe, i2) - aTabV2d(k).X() * Coeff;
            //            myPoles(cl1, i2plus1) = myPoles(classe, i2plus1) - aTabV2d(k).Y() * Coeff;
            //            i2 += 2;
            //        }

            //        for (i = 1; i <= classe; i++)
            //        {
            //            Coeff = M(i, classe); Coeff2 = M(i, cl1);
            //            for (k = 1; k <= nbcol; k++)
            //            {
            //                B(i, k) -= myPoles(classe, k) * Coeff + myPoles(cl1, k) * Coeff2;
            //            }
            //        }
            //    }


            //    if (bdeb <= bfin)
            //    {
            //        math_Matrix B2(bdeb, bfin, 1, B.UpperCol(), 0.0);

            //        for (i = bdeb; i <= bfin; i++)
            //        {
            //            for (j = 1; j <= classe; j++)
            //            {
            //                Coeff = M(i, j);
            //                for (k = 1; k <= nbcol; k++)
            //                {
            //                    B2(i, k) += B(j, k) * Coeff;
            //                }
            //            }
            //        }

            //        // Resolution:
            //        // ===========
            //        math_Matrix IBP(bdeb, bfin, bdeb, bfin);

            //        // dans IBPMatrix at IBTMatrix ne sont stockees que les resultats pour
            //        // une classe inferieure ou egale a 26 (pour l instant du moins.)

            //        if (bdeb == 2 && bfin == classe - 1 && classe <= 26)
            //        {
            //            IBPMatrix(classe, IBP);
            //        }
            //        else if (bdeb == 3 && bfin == classe - 2 && classe <= 26)
            //        {
            //            IBTMatrix(classe, IBP);
            //        }
            //        else
            //        {
            //            math_Matrix MP(1, classe, bdeb, bfin);
            //            for (i = 1; i <= classe; i++)
            //            {
            //                for (j = bdeb; j <= bfin; j++)
            //                {
            //                    MP(i, j) = M(i, j);
            //                }
            //            }
            //            math_Matrix IBP1(bdeb, bfin, bdeb, bfin);
            //            IBP1 = MP.Transposed() * MP;
            //            IBP = IBP1.Inverse();
            //        }

            //        myDone = true;
            //        for (i = bdeb; i <= bfin; i++)
            //        {
            //            for (j = bdeb; j <= bfin; j++)
            //            {
            //                IBPij = IBP(i, j);
            //                for (k = 1; k <= nbcol; k++)
            //                {
            //                    myPoles(i, k) += IBPij * B2(j, k);
            //                }
            //            }
            //        }
            //    }
            //}
        }
    }
}