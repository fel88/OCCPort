using System;
using static System.Net.Mime.MediaTypeNames;

namespace OCCPort
{
    public class gp_Dir2d
    {
        private double dU;
        private double dV;

        gp_XY coord;
        //! For this unit vector, returns its two coordinates as a number pair.
        //! Comparison between Directions
        //! The precision value is an input data.
        public gp_XY XY() { return coord; }

        public gp_Dir2d(double theXv, double theYv)
        {
            double aD = Math.Sqrt(theXv * theXv + theYv * theYv);
            //Standard_ConstructionError_Raise_if(aD <= gp::Resolution(), "gp_Dir2d() - input vector has zero norm");
            coord.SetX(theXv / aD);
            coord.SetY(theYv / aD);
        }
    }
}