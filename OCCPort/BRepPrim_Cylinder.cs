using System;

namespace OCCPort.Tester
{
    internal class BRepPrim_Cylinder : BRepPrim_Revolution
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

        private void SetMeridian()
        {
            throw new NotImplementedException();
        }
    }

    //! Implement  the OneAxis algorithm   for a revolution
    //! surface.
    class BRepPrim_Revolution : BRepPrim_OneAxis

    {
        public BRepPrim_Revolution(gp_Ax2 position, int v, double height)
        {
        }
    }

}