using System.Reflection.Metadata;
using System.Security.Cryptography;

namespace OCCPort
{
    public class ProjLib_Function : AppCont_Function
    {
        double myU1, myU2, myV1, myV2;
        bool UCouture, VCouture;
        Adaptor3d_Curve myCurve;
        Adaptor3d_Surface mySurface;
        bool[] myIsPeriodic = new bool[2];
        double[] myPeriod = new double[2];

        public static void Function_SetUVBounds(ref double
             myU1,
                   ref double myU2,
                   ref double myV1,
                   ref double myV2,
                   ref bool UCouture,
                   ref bool VCouture,

                   Adaptor3d_Curve myCurve,
                   Adaptor3d_Surface mySurface)
        {
            double W1, W2, W;
            gp_Pnt P1, P2, P;
            //
            W1 = myCurve.FirstParameter();
            W2 = myCurve.LastParameter();
            W = 0.5 * (W1 + W2);
            // on ouvre l`intervalle
            // W1 += 1.0e-9;
            // W2 -= 1.0e-9;
            P1 = myCurve.Value(W1);
            P2 = myCurve.Value(W2);
            P = myCurve.Value(W);

            switch (mySurface._GetType())
            {

            }
        }
        public override double FirstParameter()
        {
            return (myCurve.FirstParameter());
        }

        public override double LastParameter()
        {
            return (myCurve.LastParameter());
        }

        public override bool Value(double theU, NCollection_Array1<gp_Pnt2d> thePnt2d, NCollection_Array1<gp_Pnt> thePnt)
        {
            throw new System.NotImplementedException();
        }

        public override bool D1(double theU, NCollection_Array1<gp_Vec2d> theVec2d, NCollection_Array1<gp_Vec> theVec)
        {
            throw new System.NotImplementedException();
        }

        

        public ProjLib_Function(Adaptor3d_Curve C, Adaptor3d_Surface S)
        {
            myCurve = (C);
            mySurface = (S);
            myU1 = (0.0);
            myU2 = (0.0);
            myV1 = (0.0);
            myV2 = (0.0);
            UCouture = false;
            VCouture = false;


            myNbPnt = 0;
            myNbPnt2d = 1;
            Function_SetUVBounds(ref myU1, ref myU2, ref myV1, ref myV2, ref UCouture, ref VCouture, myCurve, mySurface);
            myIsPeriodic[0] = mySurface.IsUPeriodic();
            myIsPeriodic[1] = mySurface.IsVPeriodic();

            if (myIsPeriodic[0])
                myPeriod[0] = mySurface.UPeriod();
            else
                myPeriod[0] = 0.0;

            if (myIsPeriodic[1])
                myPeriod[1] = mySurface.VPeriod();
            else
                myPeriod[1] = 0.0;
        }
    }//! Class describing a continuous 3d and/or function f(u).
}