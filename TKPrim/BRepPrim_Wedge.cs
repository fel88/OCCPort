using TKMath;

namespace TKPrim
{
    //! Provides constructors without Builders.
    public class BRepPrim_Wedge : BRepPrim_GWedge
    {
        public BRepPrim_Wedge(gp_Ax2 Axes, double dx, double dy, double dz)
            : base(new BRepPrim_Builder(), Axes, dx, dy, dz)
        {


        }



    }

}
