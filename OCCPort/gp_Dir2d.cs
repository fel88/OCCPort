using System;
using static System.Net.Mime.MediaTypeNames;

namespace OCCPort
{
    public class gp_Dir2d
    {
        private double dU;
        private double dV;
        //! Computes the scalar product
        public double Dot( gp_Dir2d theOther)  { return coord.Dot(theOther.coord); }
        public void  Reverse() { coord.Reverse(); }

        gp_XY coord;
        //! For this unit vector, returns its two coordinates as a number pair.
        //! Comparison between Directions
        //! The precision value is an input data.
        public gp_XY XY() { return coord; }
        public bool IsParallel(gp_Dir2d theOther,
                                               double theAngularTolerance)
        {
            double anAng = Angle(theOther);
            if (anAng < 0)
            {
                anAng = -anAng;
            }
            return anAng <= theAngularTolerance || Math.PI - anAng <= theAngularTolerance;
        }

        public double Angle(gp_Dir2d Other)
        {
            //    Commentaires :
            //    Au dessus de 45 degres l'arccos donne la meilleur precision pour le
            //    calcul de l'angle. Sinon il vaut mieux utiliser l'arcsin.
            //    Les erreurs commises sont loin d'etre negligeables lorsque l'on est
            //    proche de zero ou de 90 degres.
            //    En 2D les valeurs angulaires sont comprises entre -PI et PI
            double Cosinus = coord.Dot(Other.coord);
            double Sinus = coord.Crossed(Other.coord);
            if (Cosinus > -0.70710678118655 && Cosinus < 0.70710678118655)
            {
                if (Sinus > 0.0) return Math.Acos(Cosinus);
                else return -Math.Acos(Cosinus);
            }
            else
            {
                if (Cosinus > 0.0) return Math.Asin(Sinus);
                else
                {
                    if (Sinus > 0.0) return Math.PI - Math.Asin(Sinus);
                    else return -Math.PI - Math.Asin(Sinus);
                }
            }
        }

        public gp_Dir2d(double theXv, double theYv)
        {
            double aD = Math.Sqrt(theXv * theXv + theYv * theYv);
            //Standard_ConstructionError_Raise_if(aD <= gp::Resolution(), "gp_Dir2d() - input vector has zero norm");
            coord.SetX(theXv / aD);
            coord.SetY(theYv / aD);
        }
    }
}