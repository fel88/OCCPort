using System;

namespace OCCPort
{
    public class Bnd_B2d
    {
        internal void Add(gp_Pnt2d thePnt)
        {
            Add(thePnt.XY());
        }

        //! Extend the Box by the absolute value of theDiff.
      public  void Enlarge( double aDiff)
        {
             double aD = Math.Abs(aDiff);
            myHSize[0] += aD;
            myHSize[1] += aD;
        }


        /**
         * Check if the box is empty.
         */
        public bool IsVoid()
        {
            return (myHSize[0] < -1e-5);
        }
        public void Add(gp_XY thePnt)
        {
            if (IsVoid())
            {
                myCenter[0] = (double)(thePnt.X());
                myCenter[1] = (double)(thePnt.Y());
                myHSize[0] = 0.0;
                myHSize[1] = 0.0;
            }
            else
            {
                double[]aDiff = {
      (thePnt.X()) - myCenter[0],
      (thePnt.Y()) - myCenter[1]
    };
                if (aDiff[0] > myHSize[0])
                {
                     double aShift = (aDiff[0] - myHSize[0]) / 2;
                    myCenter[0] += aShift;
                    myHSize[0] += aShift;
                }
                else if (aDiff[0] < -myHSize[0])
                {
                    double aShift = (aDiff[0] + myHSize[0]) / 2;
                    myCenter[0] += aShift;
                    myHSize[0] -= aShift;
                }
                if (aDiff[1] > myHSize[1])
                {
                     double aShift = (aDiff[1] - myHSize[1]) / 2;
                    myCenter[1] += aShift;
                    myHSize[1] += aShift;
                }
                else if (aDiff[1] < -myHSize[1])
                {
                     double aShift = (aDiff[1] + myHSize[1]) / 2;
                    myCenter[1] += aShift;
                    myHSize[1] -= aShift;
                }
            }
        }

        //! Query a box corner: (Center - HSize). You must make sure that
        //! the box is NOT VOID (see IsVoid()), otherwise the method returns
        //! irrelevant result.       
        internal gp_XY CornerMin()
        {
            return new gp_XY(myCenter[0] - myHSize[0], myCenter[1] - myHSize[1]);
        }
        public gp_XY CornerMax()
        {
            return new gp_XY(myCenter[0] + myHSize[0], myCenter[1] + myHSize[1]);
        }

        double[] myCenter = new double[2];
        double[] myHSize = new double[2];
    }
}