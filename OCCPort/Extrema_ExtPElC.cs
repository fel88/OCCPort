using System;

namespace OCCPort
{
    public class Extrema_ExtPElC
    {



        bool myDone;
        int myNbExt;
        double[] mySqDist = new double[4];
        bool[] myIsMin = new bool[4];
        Extrema_POnCurv[] myPoint = new Extrema_POnCurv[4];
        internal bool IsDone()
        {
            return myDone;
        }

        internal bool IsMin(int N)
        {
            if ((N < 1) || (N > NbExt())) { throw new Standard_OutOfRange(); }
            return myIsMin[N - 1];
        }

        internal int NbExt()
        {
            if (!IsDone()) { throw new StdFail_NotDone(); }
            return myNbExt;
        }

        internal void Perform(gp_Pnt P, gp_Lin L, double Tol, double Uinf, double Usup)
        {
            myDone = false;
            myNbExt = 0;
            gp_Vec V1 = new gp_Vec(L.Direction());
            gp_Pnt OR = L.Location();
            gp_Vec V = new gp_Vec(OR, P);
            double Mydist = V1.Dot(V);
            if ((Mydist >= Uinf - Tol) &&
                (Mydist <= Usup + Tol))
            {

                gp_Pnt MyP = OR.Translated(Mydist * V1);
                Extrema_POnCurv MyPOnCurve = new Extrema_POnCurv(Mydist, MyP);
                mySqDist[0] = P.SquareDistance(MyP);
                myPoint[0] = MyPOnCurve;
                myIsMin[0] = true;
                myNbExt = 1;
                myDone = true;
            }
        }

        internal Extrema_POnCurv Point(int N)
        {
            if ((N < 1) || (N > NbExt())) { throw new Standard_OutOfRange(); }
            return myPoint[N - 1];
        }

        internal double SquareDistance(int N)
        {
            if ((N < 1) || (N > NbExt())) { throw new Standard_OutOfRange(); }
            return mySqDist[N - 1];
        }
    }
}