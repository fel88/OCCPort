
using OCCPort.Common;
using System.Security.Cryptography;
using TKernel;
using TKG3d;
using TKMath;

namespace TKGeomBase
{
    public class Extrema_GExtPC : IExtrema_ExtPC
    {
        //! It calculates all the distances.
        //! The function F(u)=distance(P,C(u)) has an extremum
        //! when g(u)=dF/du=0. The algorithm searches all the
        //! zeros inside the definition range of the curve.
        //! Tol is used to decide to stop the
        //! iterations according to the following condition:
        //! if n is the number of iterations,
        //! the algorithm stops when abs(F(Un)-F(Un-1)) < Tol.
        public Extrema_GExtPC(gp_Pnt P, Adaptor3d_Curve C, double TolF = 1.0e-10)
        {

            Initialize(C, C.FirstParameter(),
                   C.LastParameter(), TolF);
            Perform(P);
        }

        public Extrema_POnCurv Point(int N)
        {
            if ((N < 1) || (N > NbExt()))
                throw new Standard_OutOfRange();

            return mypoint.Value(N);
        }

        public void Perform(gp_Pnt P)
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
                Pf = myC.Value(myuinf);
                mydist1 = P.SquareDistance(Pf);
            }

            if (Precision.IsInfinite(myusup))
                mydist2 = Standard_Real.RealLast();

            else
            {
                Pl = myC.Value(myusup);
                mydist2 = P.SquareDistance(Pl);
            }

            var aCurve = myC;

            switch (type)
            {
                case GeomAbs_CurveType.GeomAbs_Circle:
                    {
                        myExtPElC.Perform(P, aCurve.Circle(), t3d, myuinf, myusup);
                    }
                    break;
            }


            // Postprocessing.
            if (type == GeomAbs_CurveType.GeomAbs_BSplineCurve ||
                type == GeomAbs_CurveType.GeomAbs_OffsetCurve ||
                type == GeomAbs_CurveType.GeomAbs_OtherCurve)
            {
                //// Additional checking if the point is on the first or last point of the curve
                //// and does not added yet.
                //if (mydist1 < Precision::SquareConfusion() ||
                //    mydist2 < Precision::SquareConfusion())
                //{
                //    Standard_Boolean isFirstAdded = Standard_False;
                //    Standard_Boolean isLastAdded = Standard_False;
                //    Standard_Integer aNbPoints = mypoint.Length();
                //    for (i = 1; i <= aNbPoints; i++)
                //    {
                //        U = mypoint.Value(i).Parameter();
                //        if (Abs(U - myuinf) < mytolu)
                //            isFirstAdded = Standard_True;
                //        else if (Abs(myusup - U) < mytolu)
                //            isLastAdded = Standard_True;
                //    }
                //    if (!isFirstAdded && mydist1 < Precision::SquareConfusion())
                //    {
                //        mySqDist.Prepend(mydist1);
                //        myismin.Prepend(Standard_True);
                //        mypoint.Prepend(ThePOnC(myuinf, Pf));
                //    }
                //    if (!isLastAdded && mydist2 < Precision::SquareConfusion())
                //    {
                //        mySqDist.Append(mydist2);
                //        myismin.Append(Standard_True);
                //        mypoint.Append(ThePOnC(myusup, Pl));
                //    }
                //    mydone = Standard_True;
                //}
            }
            else
            {
                // In analytical case
                mydone = myExtPElC.IsDone();
                if (mydone)
                {
                    NbExt = myExtPElC.NbExt();
                    for (i = 1; i <= NbExt; i++)
                    {
                        // Verification de la validite des parametres:
                        var PC = myExtPElC.Point(i);
                        U = PC.Parameter();
                        if (myC.IsPeriodic())
                        {
                            U = ElCLib.InPeriod(U, myuinf, myuinf + myC.Period());
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

        void Initialize(Adaptor3d_Curve C,
             double Uinf,
             double Usup,
             double TolF)
        {
            myC = C;
            myintuinf = myuinf = Uinf;
            myintusup = myusup = Usup;
            mytolf = TolF;
            mytolu = C.Resolution(Precision.Confusion());
            type = C._GetType();
            mydone = false;
            mydist1 = Standard_Real.RealLast();
            mydist2 = Standard_Real.RealLast();
            mysample = 17;
        }

        protected TColStd_SequenceOfReal mySqDist = new TColStd_SequenceOfReal();

        Adaptor3d_Curve myC;
        gp_Pnt Pf;
        gp_Pnt Pl;
        Extrema_ExtPElC myExtPElC = new Extrema_ExtPElC();
        Extrema_SequenceOfPOnCurv mypoint = new Extrema_SequenceOfPOnCurv();
        bool mydone;
        TColStd_SequenceOfBoolean myismin = new TColStd_SequenceOfBoolean();

        double mydist2;
        double mydist1;
        //Extrema_EPCOfExtPC myExtPC;
        double mytolu;
        double mytolf;
        int mysample;
        double myintuinf;
        double myintusup;
        double myuinf;
        double myusup;
        GeomAbs_CurveType type;



        public int NbExt()
        {
            if (!IsDone()) throw new StdFail_NotDone();
            return mySqDist.Length();
        }

        public double SquareDistance(int N)
        {
            if ((N < 1) || (N > NbExt())) throw new Standard_OutOfRange();
            return mySqDist.Value(N);
        }
        public bool IsDone()
        {
            return mydone;
        }

    }
}
