using System;

namespace OCCPort
{
    public class Extrema_LocateExtPC
    {

        public Extrema_LocateExtPC()
        {
            myC = (null);
            mydist2 = (0.0);
            myismin = (false);
            myDone = false;
            myumin = (0.0);
            myusup = (0.0);
            mytol = (0.0);
            type = GeomAbs_CurveType.GeomAbs_OtherCurve;
            numberext = 0;

        }
        //! sets the fields of the algorithm.
        public void Initialize(Adaptor3d_Curve C, double Umin, double Usup, double TolF)
        {
            myC =  C;
            mytol = TolF;
            myumin = Umin;
            myusup = Usup;
            type = Extrema_CurveTool._GetType(C);
            double tolu = Extrema_CurveTool.Resolution(C, Precision.Confusion());
            if ((type == GeomAbs_CurveType.GeomAbs_BSplineCurve) ||
                (type == GeomAbs_CurveType.GeomAbs_BezierCurve) ||
                (type == GeomAbs_CurveType.GeomAbs_OffsetCurve) ||
                (type == GeomAbs_CurveType.GeomAbs_OtherCurve))
            {
                myLocExtPC.Initialize(C, Umin, Usup, tolu);
            }
            else
            {
                myExtremPC.Initialize(C, Umin, Usup, tolu);
            }
        }  //! Returns True if the distance is found.
        public bool IsDone()
        {
            return myDone;
        }

        internal void Perform(gp_Pnt P, double U0)
        {
            int i, i1, i2, inter;
            double Par, valU, valU2 = Standard_Real.RealLast(),
              local_u0;
            double myintuinf = 0, myintusup = 0;
            local_u0 = U0;
            switch (type)
            {
                //case GeomAbs_OtherCurve:
                //case GeomAbs_OffsetCurve:
                //case GeomAbs_BSplineCurve:
                //    {
                //        // La recherche de l extremum est faite intervalle continu C2 par
                //        // intervalle continu C2 de la courbe
                //        Standard_Integer n = TheCurveTool::NbIntervals(*((TheCurve*)myC), GeomAbs_C2);
                //        TColStd_Array1OfReal theInter(1, n + 1);
                //        TheCurveTool::Intervals(*((TheCurve*)myC), theInter, GeomAbs_C2);
                //        //
                //        //  be gentle with the caller 
                //        //
                //        if (local_u0 < myumin)
                //        {
                //            local_u0 = myumin;
                //        }
                //        else if (local_u0 > myusup)
                //        {
                //            local_u0 = myusup;
                //        }
                //        // Recherche de l intervalle ou se trouve U0
                //        Standard_Boolean found = Standard_False;
                //        inter = 1;
                //        while (!found && inter <= n)
                //        {
                //            // Intervalle commun a l intervalle C2 courant de la courbe et a
                //            // l intervalle total de recherche de l'extremum (hla : au cas ou
                //            // myintuinf > myintusup, c est que les 2 intervalles ne s intersectent
                //            // pas, mais il n'y avait aucune raison de sortir en "return")
                //            myintuinf = Max(theInter(inter), myumin);
                //            myintusup = Min(theInter(inter + 1), myusup);
                //            if ((local_u0 >= myintuinf) && (local_u0 < myintusup)) found = Standard_True;
                //            inter++;
                //        }

                //        if (found) inter--; //IFV 16.06.00 - inter is increased after found!

                //        // Essai sur l intervalle trouve
                //        myLocExtPC.Initialize((*((TheCurve*)myC)), myintuinf,
                //          myintusup, mytol);
                //        myLocExtPC.Perform(P, local_u0);
                //        myDone = myLocExtPC.IsDone();
                //        if (myDone)
                //        {
                //            mypp = myLocExtPC.Point();
                //            myismin = myLocExtPC.IsMin();
                //            mydist2 = myLocExtPC.SquareDistance();
                //        }
                //        else
                //        {
                //            Standard_Integer k = 1;
                //            // Essai sur les intervalles alentours:
                //            i1 = inter;
                //            i2 = inter;
                //            Standard_Real s1inf, s2inf, s1sup, s2sup;
                //            ThePoint P1;
                //            TheVector V1;
                //            TheCurveTool::D1(*((TheCurve*)myC), myintuinf, P1, V1);
                //            s2inf = (TheVector(P, P1) * V1);
                //            TheCurveTool::D1(*((TheCurve*)myC), myintusup, P1, V1);
                //            s1sup = (TheVector(P, P1) * V1);


                //            while (!myDone && (i2 > 0) && (i1 <= n))
                //            {
                //                i1 = inter + k;
                //                i2 = inter - k;
                //                if (i1 <= n)
                //                {
                //                    myintuinf = Max(theInter(i1), myumin);
                //                    myintusup = Min(theInter(i1 + 1), myusup);
                //                    if (myintuinf < myintusup)
                //                    {
                //                        TheCurveTool::D1(*((TheCurve*)myC), myintuinf, P1, V1);
                //                        s2sup = (TheVector(P, P1) * V1);
                //                        if (Precision::IsInfinite(s2sup) || Precision::IsInfinite(s1sup))
                //                        {
                //                            break;
                //                        }
                //                        if (s1sup * s2sup <= RealEpsilon())
                //                        {
                //                            // extremum:
                //                            myDone = Standard_True;
                //                            mypp.SetValues(myintuinf, P1);
                //                            myismin = (s1sup <= 0.0);
                //                            mydist2 = P.SquareDistance(P1);
                //                            break;
                //                        }

                //                        TheCurveTool::D1(*((TheCurve*)myC), myintusup, P1, V1);
                //                        s1sup = (TheVector(P, P1) * V1);
                //                        myLocExtPC.Initialize((*((TheCurve*)myC)), myintuinf,
                //                          myintusup, mytol);
                //                        myLocExtPC.Perform(P, (myintuinf + myintusup) * 0.5);
                //                        myDone = myLocExtPC.IsDone();
                //                        if (myDone)
                //                        {
                //                            mypp = myLocExtPC.Point();
                //                            myismin = myLocExtPC.IsMin();
                //                            mydist2 = myLocExtPC.SquareDistance();
                //                            break;
                //                        }
                //                    }
                //                }

                //                if (i2 > 0)
                //                {
                //                    myintuinf = Max(theInter(i2), myumin);
                //                    myintusup = Min(theInter(i2 + 1), myusup);
                //                    if (myintuinf < myintusup)
                //                    {
                //                        TheCurveTool::D1(*((TheCurve*)myC), myintusup, P1, V1);
                //                        s1inf = (TheVector(P, P1) * V1);
                //                        if (Precision::IsInfinite(s2inf) || Precision::IsInfinite(s1inf))
                //                        {
                //                            break;
                //                        }
                //                        if (s1inf * s2inf <= RealEpsilon())
                //                        {
                //                            // extremum:
                //                            myDone = Standard_True;
                //                            mypp.SetValues(myintusup, P1);
                //                            myismin = (s1inf <= 0.0);
                //                            mydist2 = P.SquareDistance(P1);
                //                            break;
                //                        }

                //                        TheCurveTool::D1(*((TheCurve*)myC), myintuinf, P1, V1);
                //                        s2inf = (TheVector(P, P1) * V1);
                //                        myLocExtPC.Initialize((*((TheCurve*)myC)), myintuinf,
                //                          myintusup, mytol);
                //                        myLocExtPC.Perform(P, (myintuinf + myintusup) * 0.5);
                //                        myDone = myLocExtPC.IsDone();

                //                        if (myDone)
                //                        {
                //                            mypp = myLocExtPC.Point();
                //                            myismin = myLocExtPC.IsMin();
                //                            mydist2 = myLocExtPC.SquareDistance();
                //                            break;
                //                        }
                //                    }
                //                }

                //                k++;
                //            }
                //        }
                //    }

                //    break;

                //case GeomAbs_BezierCurve:
                //    {
                //        myLocExtPC.Perform(P, U0);
                //        myDone = myLocExtPC.IsDone();
                //    }

                //    break;
                default:
                    {
                        myExtremPC.Perform(P);
                        numberext = 0;
                        if (myExtremPC.IsDone())
                        {
                            for (i = 1; i <= myExtremPC.NbExt(); i++)
                            {
                                Par = myExtremPC.Point(i).Parameter();
                                valU = Math.Abs(Par - U0);
                                if (valU <= valU2)
                                {
                                    valU2 = valU;
                                    numberext = i;
                                    myDone = true;
                                }
                            }
                        }

                        if (numberext == 0)
                            myDone = false;

                        break;
                    }
            }
        }

        internal Extrema_POnCurv Point()
        {
            if (!IsDone())
            {
                throw new StdFail_NotDone();
            }
            if (type == GeomAbs_CurveType.GeomAbs_BezierCurve)
            {
                return myLocExtPC.Point();
            }
            else if (type == GeomAbs_CurveType.GeomAbs_BSplineCurve ||
                    type == GeomAbs_CurveType.GeomAbs_OffsetCurve ||
                    type == GeomAbs_CurveType.GeomAbs_OtherCurve)
            {
                return mypp;
            }
            return myExtremPC.Point(numberext);
        }

        bool myDone;
        Extrema_POnCurv mypp;
        object myC;
        double mydist2;
        bool myismin;

        double myumin;
        double myusup;
        double mytol;
        Extrema_LocEPCOfLocateExtPC myLocExtPC = new Extrema_LocEPCOfLocateExtPC();
        Extrema_ELPCOfLocateExtPC myExtremPC = new Extrema_ELPCOfLocateExtPC();
        GeomAbs_CurveType type;
        int numberext;

    }
}