using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OCCPort
{
    //! Defines  a non-persistent 2D cartesian point.

    public struct gp_Pnt2d
    {

        gp_XY coord;
        //=======================================================================
        public double SquareDistance(gp_Pnt2d theOther)
        {
            gp_XY aXY = theOther.coord;
            double aX = coord.X() - aXY.X();
            double aY = coord.Y() - aXY.Y();
            return (aX * aX + aY * aY);
        }

        public double X()
           => coord.X();
        public double Y()
           => coord.Y();
        //! For this point, returns its two coordinates as a number pair.
        public gp_XY XY() { return coord; }

        //! Creates a  point with its 2 cartesian's coordinates : theXp, theYp.
        public gp_Pnt2d(double theXp, double theYp)

        {
            coord = new gp_XY(theXp, theYp);

        }

        public gp_Pnt2d(gp_XY gp_XY) : this()
        {
            coord = new gp_XY(gp_XY.X(), gp_XY.Y());
        }
    }
}