using System;

namespace OCCPort
{
    public class Extrema_POnCurv
    {

        public Extrema_POnCurv()
        {
            myU = 0;
            myP = new gp_Pnt();
        }
        public Extrema_POnCurv(double U, gp_Pnt P)
        {
            myP = P;
            myU = U;
        }
        public void SetValues(double U, gp_Pnt P)
        {
            myP = P;
            myU = U;
        }
        //! Returns the point.
        public gp_Pnt Value()
        {
            return myP;
        }

        //! Returns the parameter on the curve.
        public double Parameter()
        {
            return myU;
        }
        double myU;
        gp_Pnt myP;


    }
}