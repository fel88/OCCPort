using System;
using static System.Net.Mime.MediaTypeNames;

namespace OCCPort
{
    internal class gp_Dir2d
    {
        private double dU;
        private double dV;

        gp_XY coord;

        public gp_Dir2d(double theXv, double theYv)
        {
            double aD = Math.Sqrt(theXv * theXv + theYv * theYv);
            //Standard_ConstructionError_Raise_if(aD <= gp::Resolution(), "gp_Dir2d() - input vector has zero norm");
            coord.SetX(theXv / aD);
            coord.SetY(theYv / aD);
        }
    }
}