using OCCPort.Common;
using TKG2d;
using TKG3d;
using TKMath;

namespace TKPrim
{
    //! Cylinder primitive.
    public class BRepPrim_Cylinder : BRepPrim_Revolution
    {
        public BRepPrim_Cylinder(gp_Ax2 Position,
                      double Radius,

                      double Height)
            : base(Position, 0, Height)
        {

            myRadius = (Radius);
            SetMeridian();
        }

        double myRadius; //!< cylinder radius

        public BRepPrim_Cylinder(double R,
                      double H) : base(gp.XOY(), 0, H)
        {

            myRadius = (R);
            SetMeridian();
        }

        private void SetMeridian()
        {
            gp_Vec V = Axes().XDirection();
            V.Multiply(myRadius);
            gp_Ax1 A = Axes().Axis();
            A.Translate(V);
            Geom_Line L = new Geom_Line(A);
            Geom2d_Line L2d = new Geom2d_Line(new gp_Pnt2d(myRadius, 0), new gp_Dir2d(0, 1));
            Meridian(L, L2d);
        }
    }
}
