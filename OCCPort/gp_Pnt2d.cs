using static System.Net.Mime.MediaTypeNames;

namespace OCCPort
{
    public struct gp_Pnt2d
    {

        gp_XY coord;

        //! Creates a  point with its 2 cartesian's coordinates : theXp, theYp.
        public gp_Pnt2d(double theXp, double theYp)

        {
            coord = new gp_XY(theXp, theYp);

        }

    }
}