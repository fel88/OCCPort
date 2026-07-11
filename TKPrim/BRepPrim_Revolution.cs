using System.Reflection.Metadata;
using TKG2d;
using TKG3d;
using TKMath;

namespace TKPrim
{
    //! Implement  the OneAxis algorithm   for a revolution
    //! surface.
    public class BRepPrim_Revolution : BRepPrim_OneAxis

    {
        public BRepPrim_Revolution(gp_Ax2 position, int v, double height)
        {
        }
        public void Meridian(Geom_Curve M,
                     Geom2d_Curve PM)
        {
            myMeridian = M;
            myPMeridian = PM;
        }

        Geom_Curve myMeridian;     
        Geom2d_Curve myPMeridian;
    }

}
