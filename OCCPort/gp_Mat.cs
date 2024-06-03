using System;

namespace OCCPort
{
    internal class gp_Mat
    {
        double[][] myMat = new double[3][];
        //! creates  a matrix with null coefficients.
        public gp_Mat(double theA11, double theA12, double theA13,
                        double theA21, double theA22, double theA23,
                        double theA31, double theA32, double theA33)
        {

            for (int i = 0; i < 3; i++)
            {
                myMat[i] = new double[3];
            }
            myMat[0][0] = theA11;
            myMat[0][1] = theA12;
            myMat[0][2] = theA13;
            myMat[1][0] = theA21;
            myMat[1][1] = theA22;
            myMat[1][2] = theA23;
            myMat[2][0] = theA31;
            myMat[2][1] = theA32;
            myMat[2][2] = theA33;
        }

        public gp_Mat()
        {
            for (int i = 0; i < 3; i++)
            {
                myMat[i] = new double[3];
            }
            myMat[0][0] = myMat[0][1] = myMat[0][2] =
            myMat[1][0] = myMat[1][1] = myMat[1][2] =
            myMat[2][0] = myMat[2][1] = myMat[2][2] = 0.0;
        }
        internal void Multiply(gp_Mat theOther)
        {
            double aT00 = myMat[0][0] * theOther.myMat[0][0] + myMat[0][1] * theOther.myMat[1][0] + myMat[0][2] * theOther.myMat[2][0];
            double aT01 = myMat[0][0] * theOther.myMat[0][1] + myMat[0][1] * theOther.myMat[1][1] + myMat[0][2] * theOther.myMat[2][1];
            double aT02 = myMat[0][0] * theOther.myMat[0][2] + myMat[0][1] * theOther.myMat[1][2] + myMat[0][2] * theOther.myMat[2][2];
            double aT10 = myMat[1][0] * theOther.myMat[0][0] + myMat[1][1] * theOther.myMat[1][0] + myMat[1][2] * theOther.myMat[2][0];
            double aT11 = myMat[1][0] * theOther.myMat[0][1] + myMat[1][1] * theOther.myMat[1][1] + myMat[1][2] * theOther.myMat[2][1];
            double aT12 = myMat[1][0] * theOther.myMat[0][2] + myMat[1][1] * theOther.myMat[1][2] + myMat[1][2] * theOther.myMat[2][2];
            double aT20 = myMat[2][0] * theOther.myMat[0][0] + myMat[2][1] * theOther.myMat[1][0] + myMat[2][2] * theOther.myMat[2][0];
            double aT21 = myMat[2][0] * theOther.myMat[0][1] + myMat[2][1] * theOther.myMat[1][1] + myMat[2][2] * theOther.myMat[2][1];
            double aT22 = myMat[2][0] * theOther.myMat[0][2] + myMat[2][1] * theOther.myMat[1][2] + myMat[2][2] * theOther.myMat[2][2];
            myMat[0][0] = aT00;
            myMat[0][1] = aT01;
            myMat[0][2] = aT02;
            myMat[1][0] = aT10;
            myMat[1][1] = aT11;
            myMat[1][2] = aT12;
            myMat[2][0] = aT20;
            myMat[2][1] = aT21;
            myMat[2][2] = aT22;
        }

        internal void SetRotation(gp_XYZ theAxis, double theAng)
        {
            //    Rot = I + sin(Ang) * M + (1. - cos(Ang)) * M*M
            //    avec  M . XYZ = Axis ^ XYZ
            gp_XYZ aV = theAxis.Normalized();
            SetCross(aV);
            Multiply(Math.Sin(theAng));
            gp_Mat aTemp = new gp_Mat();
            aTemp.SetScale(1.0);
            Add(aTemp);
            double A = aV.X();
            double B = aV.Y();
            double C = aV.Z();
            aTemp.SetRow(1, new gp_XYZ(-C * C - B * B, A * B, A * C));
            aTemp.SetRow(2, new gp_XYZ(A * B, -A * A - C * C, B * C));
            aTemp.SetRow(3, new gp_XYZ(A * C, B * C, -A * A - B * B));
            aTemp.Multiply(1.0 - Math.Cos(theAng));
            Add(aTemp);
        }

        private void SetCross(gp_XYZ theRef)
        {
            double X = theRef.X();
            double Y = theRef.Y();
            double Z = theRef.Z();
            myMat[0][0] = myMat[1][1] = myMat[2][2] = 0.0;
            myMat[0][1] = -Z;
            myMat[0][2] = Y;
            myMat[1][2] = -X;
            myMat[1][0] = Z;
            myMat[2][0] = -Y;
            myMat[2][1] = X;
        }

        private void SetScale(double theS)
        {
            myMat[0][0] = myMat[1][1] = myMat[2][2] = theS;
            myMat[0][1] = myMat[0][2] = myMat[1][0] = myMat[1][2] = myMat[2][0] = myMat[2][1] = 0.0;
        }

        private void Add(gp_Mat theOther)
        {
            myMat[0][0] += theOther.myMat[0][0];
            myMat[0][1] += theOther.myMat[0][1];
            myMat[0][2] += theOther.myMat[0][2];
            myMat[1][0] += theOther.myMat[1][0];
            myMat[1][1] += theOther.myMat[1][1];
            myMat[1][2] += theOther.myMat[1][2];
            myMat[2][0] += theOther.myMat[2][0];
            myMat[2][1] += theOther.myMat[2][1];
            myMat[2][2] += theOther.myMat[2][2];
        }

        public void Multiply(double theScalar)
        {
            myMat[0][0] *= theScalar;
            myMat[0][1] *= theScalar;
            myMat[0][2] *= theScalar;
            myMat[1][0] *= theScalar;
            myMat[1][1] *= theScalar;
            myMat[1][2] *= theScalar;
            myMat[2][0] *= theScalar;
            myMat[2][1] *= theScalar;
            myMat[2][2] *= theScalar;
        }

        private void SetRow(int theRow, gp_XYZ theValue)
        {
            if (theRow < 1 || theRow > 3)
                throw new ArgumentOutOfRangeException();

            if (theRow == 1)
            {
                myMat[0][0] = theValue.X(); myMat[0][1] = theValue.Y(); myMat[0][2] = theValue.Z();
            }
            else if (theRow == 2)
            {
                myMat[1][0] = theValue.X(); myMat[1][1] = theValue.Y(); myMat[1][2] = theValue.Z();
            }
            else
            {
                myMat[2][0] = theValue.X(); myMat[2][1] = theValue.Y(); myMat[2][2] = theValue.Z();
            }
        }

        internal double Value(int theRow, int theCol)
        {
            //Standard_OutOfRange_Raise_if(theRow < 1 || theRow > 3 || theCol < 1 || theCol > 3, " ");
            return myMat[theRow - 1][theCol - 1];
        }
    }
}