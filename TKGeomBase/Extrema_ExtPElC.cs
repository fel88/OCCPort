using OCCPort.Common;
using TKMath;

namespace TKGeomBase
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

        public void Perform(gp_Pnt P,
                   gp_Circ C,
                   double Tol,
                   double Uinf,
                   double Usup)
        /*-----------------------------------------------------------------------------
        Function:
          Find values of parameter u such as:
           - dist(P,C(u)) pass by an extrema,
           - Uinf <= u <= Usup.

        Method:
          Pass 3 stages:
          1- Projection of point P in the plane of the circle,
          2- Calculation of u solutions in [0.,2.*M_PI]:
              Let Pp, the projected point and 
                   O, the center of the circle;
             2 cases:
             - if Pp is mixed with 0, there is an infinite number of solutions;
               IsDone() renvoie Standard_False.
             - otherwise, 2 points are solutions for the complete circle:
               . Us1 = angle(OPp,OX) corresponds to the minimum,
               . let Us2 = ( Us1 + M_PI if Us1 < M_PI,
                            ( Us1 - M_PI otherwise;
                 Us2 corresponds to the maximum.
          3- Calculate the extrema in [Uinf,Usup].
        -----------------------------------------------------------------------------*/
        {
            myDone = false;
            myNbExt = 0;

            // 1- Projection of the point P in the plane of circle -> Pp ...

            gp_Pnt O = C.Location();
            gp_Vec Axe = new(C.Axis().Direction());
            gp_Vec Trsl = Axe.Multiplied(-(new gp_Vec(O, P).Dot(Axe)));
            gp_Pnt Pp = P.Translated(Trsl);

            // 2- Calculate u solutions in [0.,2.*PI] ...

            gp_Vec OPp = new(O, Pp);
            if (OPp.Magnitude() < Tol) { return; }
            double[] Usol = new double[2];
            Usol[0] = C.XAxis().Direction().AngleWithRef(OPp, Axe); // -M_PI<U1<M_PI

            double aAngTol = Precision.Angular();
            if (Usol[0] + Math.PI < aAngTol)
                Usol[0] = -Math.PI;
            else if (Usol[0] - Math.PI > -aAngTol)
                Usol[0] = Math.PI;

            Usol[1] = Usol[0] + Math.PI;

            double myuinf = Uinf;
            //Standard_Real TolU = Tol*C.Radius();
            double TolU, aR;
            aR = C.Radius();
            TolU = Precision.Infinite();
            if (aR > gp.Resolution())
            {
                TolU = Tol / aR;
            }
            //
            ElCLib.AdjustPeriodic(Uinf, Uinf + 2 * Math.PI, TolU, ref myuinf, ref Usol[0]);
            ElCLib.AdjustPeriodic(Uinf, Uinf + 2 * Math.PI, TolU, ref myuinf, ref Usol[1]);
            if (((Usol[0] - 2 * Math.PI - Uinf) < TolU) && ((Usol[0] - 2 * Math.PI - Uinf) > -TolU)) Usol[0] = Uinf;
            if (((Usol[1] - 2 * Math.PI - Uinf) < TolU) && ((Usol[1] - 2 * Math.PI - Uinf) > -TolU)) Usol[1] = Uinf;


            // 3- Calculate extrema in [Umin,Umax] ...

            gp_Pnt Cu;
            double Us;
            for (int NoSol = 0; NoSol <= 1; NoSol++)
            {
                Us = Usol[NoSol];
                if (((Uinf - Us) < TolU) && ((Us - Usup) < TolU))
                {
                    Cu = ElCLib.Value(Us, C);
                    mySqDist[myNbExt] = Cu.SquareDistance(P);
                    myIsMin[myNbExt] = (NoSol == 0);
                    myPoint[myNbExt] = new Extrema_POnCurv(Us, Cu);
                    myNbExt++;
                }
            }
            myDone = true;
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
